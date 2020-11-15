using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour, IMove, IAttack, IIdle, IShoot
{
    [SerializeField] private GameObject _target = null;
    //[SerializeField] private AudioSource reloadGun;
    private Rigidbody _targetRB = null;
    private float walkSpeed = 2f; //1.1f
    private Rigidbody rb = null;
    private Animator anim;
    private float life = 100;

    //PathFind
    private int _startNode = 0;
    private Node[] nodes; //Que tenga todos los nodos
    [SerializeField] private float smoothnessTurn = 1;
    private PathfindController _myPathfindController;
    private int wayPointIncrease = 1;
    private int nextWayPoint = 0;

    //Ammo
    private float _reloadingAmmoTime = 5;
    private const int _maxAmmo = 5;
    private int _ammoLeft = 0;

    //IsTargetNear
    private float _nearDistance = 5f;

    //Walk
    private float _walkTimeBeforeIdle = 5f;
    private float _currentWalkedTime = 0;

    private FSMController<string> _myFSMController;
    private LineOfSight _lineOfSigh = null;
    private INode _initialNode;

    //Steering
    private Pursuit pursuitSteering;

    public event Action OnShoot;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        nodes = NodesManager.Instance.GetNodes();

        RandomWithException rndWithException = new RandomWithException(0, nodes.Length, _startNode);
        var randomEndNode = rndWithException.Randomize();
        //print($"RandomEndNodeInicial = {randomEndNode} equivale a {nodes[randomEndNode]} ; CurrentNode = {nodes[_startNode]}");
        _myPathfindController = new PathfindController(nodes[_startNode], nodes[randomEndNode]);
        _myPathfindController.Execute();
        _startNode = randomEndNode;

        _ammoLeft = _maxAmmo;
        _lineOfSigh = GetComponent<LineOfSight>();

        //Steerings
        //_targetRB = _target.GetComponent<Rigidbody>();
        pursuitSteering = new Pursuit(this.transform, 2);

        //FSM
        _myFSMController = new FSMController<string>();

        IdleState<string> idle = new IdleState<string>(this);
        ReloadState<string> reloadAmmo = new ReloadState<string>(this);
        WalkState<string> walk = new WalkState<string>(this);
        PursuitState<string> pursuit = new PursuitState<string>(this);
        KillState<string> kill = new KillState<string>(this);

        idle.AddTransitionToState("walk", walk);
        idle.AddTransitionToState("pursuit", pursuit);
        idle.AddTransitionToState("kill", kill);
        idle.AddTransitionToState("reloadAmmo", reloadAmmo);

        walk.AddTransitionToState("idle", idle);
        walk.AddTransitionToState("pursuit", pursuit);
        walk.AddTransitionToState("kill", kill);
        walk.AddTransitionToState("reloadAmmo", reloadAmmo);

        kill.AddTransitionToState("idle", idle);
        kill.AddTransitionToState("walk", walk);
        kill.AddTransitionToState("pursuit", pursuit);
        kill.AddTransitionToState("reloadAmmo", reloadAmmo);

        reloadAmmo.AddTransitionToState("idle", idle);
        reloadAmmo.AddTransitionToState("walk", walk);
        reloadAmmo.AddTransitionToState("pursuit", pursuit);
        reloadAmmo.AddTransitionToState("kill", kill);

        pursuit.AddTransitionToState("idle", idle);
        pursuit.AddTransitionToState("walk", walk);
        pursuit.AddTransitionToState("kill", kill);
        pursuit.AddTransitionToState("reloadAmmo", reloadAmmo);

        _myFSMController.SetInitialState(idle);

        //TREE
        ActionNode respawn = new ActionNode(Respawn);

        QuestionNode haveAmmo = new QuestionNode(CheckAmmoLeft, kill, reloadAmmo);
        QuestionNode isTargetNear = new QuestionNode(IsTargetNear, haveAmmo, pursuit);
        QuestionNode amIExhausted = new QuestionNode(WalkedTime, idle, walk);
        QuestionNode isInSight = new QuestionNode(IsTargetInSight, isTargetNear, amIExhausted);

        _initialNode = isInSight;
    }

    private void Update()
    {
        _initialNode.Execute();
        //_myFSMController.OnUpdate();
    }

    public void KillTarget() //IATTACK 
    {
        //print("shooting");
        anim.SetBool("IsReloading", false);
        anim.SetBool("IsInSight", true);
        anim.SetInteger("Speed", 0);

        rb.velocity = Vector3.zero;
        var targ = _lineOfSigh.GetTargetReference();
        print(targ);
        var direction = pursuitSteering.GetDirection(targ);
        direction.y = transform.forward.y;
        transform.forward = direction - new Vector3(1,0,0);
        OnShoot?.Invoke();
        _ammoLeft -= 1;
    } 

    public void Move() //IMOVE
    {
        anim.SetBool("IsReloading", false);
        anim.SetBool("IsInSight", false);
        anim.SetInteger("Speed", 2);

        _currentWalkedTime += Time.deltaTime;
        var direction = GetNextPosition();
        rb.velocity = direction * walkSpeed;
        transform.forward = Vector3.Lerp(transform.forward, direction, 10 * Time.deltaTime);
    } 

    public void PursuitTarget() //IATTACK 
    {
        anim.SetInteger("Speed", 2);
        anim.SetBool("IsInSight", false);
        anim.SetBool("IsReloading", false);

        //print("persiguiendo");
        var targ = _lineOfSigh.GetTargetReference();
        rb.velocity = pursuitSteering.GetDirection(targ) * walkSpeed;
        var direction = pursuitSteering.GetDirection(targ);
        transform.forward = Vector3.Lerp(transform.forward, direction, 10 * Time.deltaTime);
    }

    public void ReloadAmmo() //IATTACK 
    {
        anim.SetBool("IsReloading", true);

        rb.velocity = Vector3.zero;
        var targ = _lineOfSigh.GetTargetReference();
        var direction = pursuitSteering.GetDirection(targ) - new Vector3(1, 0, 0);
        direction.y = transform.forward.y;
        transform.forward = Vector3.Lerp(transform.forward, direction, 5 * Time.deltaTime);
        StartCoroutine(ReloadingAmmo(_reloadingAmmoTime));
    } 

    public void Respawn() 
    {
        anim.SetBool("IsDeath", false);
    }

    public bool WalkedTime()
    {
        if(_currentWalkedTime > _walkTimeBeforeIdle)
        {
            return true;
        }
        else 
        {
            return false; 
        }
    }

    public bool IsTargetInSight() 
    {
        print(_lineOfSigh.IsTargetInSight(GameManager.Instance.bandides));
        return _lineOfSigh.IsTargetInSight(GameManager.Instance.bandides);
    }

    public bool CheckAmmoLeft()
    {
        if (_ammoLeft > 0)
        {
            return true;
        }
        else { return false; }
    }

    public bool IsTargetNear()
    {
        print(_lineOfSigh.GetDistanceToTarget(_nearDistance));
        return _lineOfSigh.GetDistanceToTarget(_nearDistance);
    }

    private Vector3 GetNextPosition() //aca
    {
        //print("GetNextPosition: nextWayPoint = " + nextWayPoint);
        var nextPointPosition = _myPathfindController.AStarResult[nextWayPoint].transform.position;
        nextPointPosition.y = transform.position.y;
        Vector3 direction = nextPointPosition - transform.position;

        if (direction.magnitude < smoothnessTurn) 
        {
            if (nextWayPoint < _myPathfindController.AStarResult.Count - 1)
            {
                nextWayPoint += wayPointIncrease;
                //print("nextWayPointIncreased = " + nextWayPoint);
            }
            else
            {
                nextWayPoint = 0;
                RandomWithException randomWithException = new RandomWithException(0, nodes.Length, _startNode);
                var randomEndNode = randomWithException.Randomize();
                //print($"RandomEndNode = {randomEndNode} equivale a {nodes[randomEndNode]} ; CurrentNode = {nodes[_startNode]}");
                _myPathfindController.EditNodes(nodes[_startNode], nodes[randomEndNode]);
                _myPathfindController.Execute();
                _startNode = randomEndNode;

            }
        }
        return direction.normalized;
    }

    public void DoIdle() //IIdle
    {
        anim.SetBool("IsDeath", false);
        anim.SetBool("IsInSight", false);
        anim.SetInteger("Speed", 0);

        StartCoroutine(WaitToRecover());
    }

    IEnumerator ReloadingAmmo(float reloadingAmmoTime)
    {
        yield return new WaitForSeconds(reloadingAmmoTime);
        _ammoLeft = _maxAmmo;
    }

    IEnumerator WaitToRecover()
    {
        yield return new WaitForSeconds(5);
        _currentWalkedTime = 0;
    }

    public void GetDamage(float _damage)
    {
        if(life - _damage <= 0)
        {
            anim.SetBool("IsDeath", true);
            life = 100;
            Respawn();
        }
        else
        {
            life -= _damage;
        }
    }
}
