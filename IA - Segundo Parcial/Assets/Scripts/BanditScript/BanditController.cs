using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditController : MonoBehaviour, IMove, IAttack, IIdle, IShoot
{
    [SerializeField] private GameObject _target;
    private Rigidbody _targetRB = null;
    private float walkSpeed = 1.5f; //1.1f
    private Rigidbody rb = null;
    private Animator anim;
    private Roulette _roulette;
    private Dictionary<float, int> _dic;
    private float life;
    [SerializeField] private Vector3 offset = Vector3.zero;     

    //Ammo
    private float _reloadingAmmoTime = 5;
    private const int _maxAmmo = 5;
    private int _ammoLeft = 0;

    //IsTargetNear
    private float _nearDistance = 5f;    

    private FSMController<string> _myFSMController;
    private LineOfSight _lineOfSigh = null;
    private INode _initialNode;

    //ObstacleAvoidance
    private ObstacleAvoidance2 _obstacleAvoidance;

    //Steering
    private Pursuit pursuitSteering;

    public event Action OnShoot;

    private void Start()
    {
        GameManager.Instance.bandides.Add(this.gameObject);
        //print("me agrego a la lista de bandidos");

        transform.position = _target.transform.position + offset;
        transform.rotation = _target.transform.rotation;

        _roulette = new Roulette();
        _dic = new Dictionary<float, int>();
        _dic.Add(1000, 75);
        _dic.Add(1250, 50);
        _dic.Add(1500, 20);
        TypeOfDamage();
        //Debug.Log(life + "mi vida es");

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.SetBool("IsDeath", true);

        _ammoLeft = _maxAmmo;
        _lineOfSigh = GetComponent<LineOfSight>();


        //Steerings
        _obstacleAvoidance = GetComponent<ObstacleAvoidance2>();
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
        ActionNode dead = new ActionNode(Dead);        

        QuestionNode haveAmmo = new QuestionNode(CheckAmmoLeft, kill, reloadAmmo);
        QuestionNode isTargetNear = new QuestionNode(IsTargetNear, haveAmmo, pursuit);
        QuestionNode isPlayerMoving = new QuestionNode(CheckTargetVelocity, walk, idle);
        QuestionNode isInSight = new QuestionNode(IsTargetInSight, isTargetNear, isPlayerMoving);
        QuestionNode iAmAlive = new QuestionNode(CheckLife, isInSight, dead);

        _initialNode = iAmAlive;       

    }

    private void Update()
    {
        _initialNode.Execute();
        //_myFSMController.OnUpdate();
    }

    public void KillTarget() //IATTACK 
    {
        print("bandido matando a target" );
        //print("shooting");
        anim.SetBool("IsReloading", false);
        anim.SetBool("IsInSight", true);
        anim.SetInteger("Speed", 0);

        rb.velocity = Vector3.zero;
        var targ = _lineOfSigh.GetTargetReference();
        //print(targ);
        var direction = pursuitSteering.GetDirection(targ);
        direction.y = 0;
        transform.forward = direction - new Vector3(-1, 0, 0);
        OnShoot?.Invoke();
        _ammoLeft -= 1;
    }

    public void Move() //IMOVE
    {
        anim.SetBool("IsReloading", false);
        anim.SetBool("IsInSight", false);
        anim.SetInteger("Speed", 2);
        //print("bandido moviendose");

        var direction = _obstacleAvoidance.GetDir();        
        direction.y = 0;
        rb.velocity = direction * walkSpeed;
        transform.forward = Vector3.Lerp(transform.forward, direction, 10 * Time.deltaTime);
    }

    public void PursuitTarget() //IATTACK 
    {
        anim.SetInteger("Speed", 2);
        anim.SetBool("IsInSight", false);
        anim.SetBool("IsReloading", false);

        //print("persiguiendo");
        //print("bandido persiguiendo");

        var targ = _lineOfSigh.GetTargetReference();
        rb.velocity = pursuitSteering.GetDirection(targ) * walkSpeed;
        var direction = pursuitSteering.GetDirection(targ);
        transform.forward = Vector3.Lerp(transform.forward, direction, 10 * Time.deltaTime);
    }

    public void ReloadAmmo() //IATTACK 
    {
        anim.SetBool("IsReloading", true);
        //print("bandido recargando");

        rb.velocity = Vector3.zero;
        var targ = _lineOfSigh.GetTargetReference();
        var direction = pursuitSteering.GetDirection(targ) - new Vector3(-1, 0, 0);
        direction.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, direction, 5 * Time.deltaTime);
        StartCoroutine(ReloadingAmmo(_reloadingAmmoTime));
    }

    public void Dead()
    {
        anim.SetBool("IsDeath", true);
        GameManager.Instance.bandides.Remove(this.gameObject);
        Destroy(gameObject, 2.3f);
    }

    public bool CheckTargetVelocity()
    {
        var targetRbRef = _target.GetComponent<Rigidbody>();
        if (targetRbRef.velocity.magnitude > 1)
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
        return _lineOfSigh.GetDistanceToTarget(_nearDistance);
    }       

    public void DoIdle() //IIdle
    {
        anim.SetBool("IsDeath", false);
        anim.SetBool("IsInSight", false);
        anim.SetInteger("Speed", 0);       
    }   

    public void ChangeTarget(GameObject _newTarget)
    {
        _target = _newTarget;
    }

    void TypeOfDamage()
    {
        life = _roulette.Run(_dic);
    }


    IEnumerator ReloadingAmmo(float reloadingAmmoTime)
    {
        yield return new WaitForSeconds(reloadingAmmoTime);
        _ammoLeft = _maxAmmo;
    }
   

    public void GetDamage(float _damage)
    {
        life -= _damage;
    }

    public bool CheckLife() 
    {
        if(life > 0)
        {
            //print("life " + life);
            return true;
        }
        else
        {
            return false;
        }
    }

}

