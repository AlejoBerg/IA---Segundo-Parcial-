using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditController : MonoBehaviour, IMove, IAttack, IIdle, IShoot
{
    [SerializeField] private GameObject _target = null;
    //[SerializeField] private AudioSource reloadGun;
    [SerializeField] private GameObject _player;
    private Rigidbody _targetRB = null;
    private float walkSpeed = 1.5f; //1.1f
    private Rigidbody rb = null;
    private Animator anim;
    private Roulette _roulette;
    private Dictionary<float, int> _dic;
    private float life;
    [SerializeField] private Vector3 offset = Vector3.zero; 
    

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
    private float _nearDistance = 8f;    

    private FSMController<string> _myFSMController;
    private LineOfSight _lineOfSigh = null;
    private INode _initialNode;

    //ObstacleAvoidance
    private ObstacleAvoidance _obstacleAvoidance;

    //Steering
    private Pursuit pursuitSteering;

    public event Action OnShoot;

    private void Awake()
    {
        transform.position = _player.transform.position + offset;
        transform.rotation = _player.transform.rotation;

        _roulette = new Roulette();
        _dic = new Dictionary<float, int>();
        _dic.Add(1000, 75);
        _dic.Add(1250, 50);
        _dic.Add(1500, 20);        
        TypeOfDamage();
       // Debug.Log(life + "mi vida es");
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        //nodes = NodesManager.Instance.GetNodes();

        //RandomWithException rndWithException = new RandomWithException(0, nodes.Length, _startNode);
        //var randomEndNode = rndWithException.Randomize();
        //print($"RandomEndNodeInicial = {randomEndNode} equivale a {nodes[randomEndNode]} ; CurrentNode = {nodes[_startNode]}");
        //_myPathfindController = new PathfindController(nodes[_startNode], nodes[randomEndNode]);
        //_myPathfindController.Execute();
        //_startNode = randomEndNode;

        _ammoLeft = _maxAmmo;
        _lineOfSigh = GetComponent<LineOfSight>();


        //Steerings
        _obstacleAvoidance = GetComponent<ObstacleAvoidance>();
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
        ActionNode evade = new ActionNode(EvadeObstacle);

        QuestionNode haveAmmo = new QuestionNode(CheckAmmoLeft, kill, reloadAmmo);
        QuestionNode isTargetNear = new QuestionNode(IsTargetNear, haveAmmo, pursuit);
        QuestionNode isPlayerMoving = new QuestionNode(PlayerVelocity, walk, idle);
        QuestionNode isInSight = new QuestionNode(IsTargetInSight, isTargetNear, isPlayerMoving);
        QuestionNode isObstacleNear = new QuestionNode(IsObstacleNear, evade, isInSight);

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
        transform.forward = direction - new Vector3(1, 0, 0);
        OnShoot?.Invoke();
        _ammoLeft -= 1;
    }

    public void Move() //IMOVE
    {
        anim.SetBool("IsReloading", false);
        anim.SetBool("IsInSight", false);
        anim.SetInteger("Speed", 2);
        
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

    public bool PlayerVelocity()
    {
        var playerRbRef = _player.GetComponent<Rigidbody>();
        if (playerRbRef.velocity.magnitude > 0)
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
        return _lineOfSigh.IsTargetInSight(GameManager.Instance.cops);
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

    public bool IsObstacleNear()
    {
        print (_obstacleAvoidance.IsObstacleNear());
        return _obstacleAvoidance.IsObstacleNear();        
    }

    private Vector3 GetNextPosition() //aca
    {
        //print("GetNextPosition: nextWayPoint = " + nextWayPoint);
        var nextPointPosition = _player.transform.position + offset;
        nextPointPosition.y = transform.position.y;
        Vector3 direction = nextPointPosition - transform.position;       
        return direction.normalized;
    }

    public void DoIdle() //IIdle
    {
        anim.SetBool("IsDeath", false);
        anim.SetBool("IsInSight", false);
        anim.SetInteger("Speed", 0);       
    }

    public void EvadeObstacle()
    {
        print("evadiendo obstaculo");
        transform.position = _obstacleAvoidance.RunObstacleAvoidance(); 
    }

    void TypeOfDamage()
    {
        life = _roulette.Run(_dic);
        Debug.Log(_roulette.Run(_dic));
    }


    IEnumerator ReloadingAmmo(float reloadingAmmoTime)
    {
        yield return new WaitForSeconds(reloadingAmmoTime);
        _ammoLeft = _maxAmmo;
    }
   

    public void GetDamage(float _damage)
    {
        if (life - _damage <= 0)
        {
            anim.SetBool("IsDeath", true);            
            Destroy(gameObject);
        }
        else
        {
            life -= _damage;
        }
    }
}

