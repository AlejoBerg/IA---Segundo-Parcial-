﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour, IMove, IAttack, IIdle, IShoot
{
    [SerializeField] private GameObject _target = null;
    [SerializeField] private AudioSource reloadGun;
    private Rigidbody _targetRB = null;
    private float walkSpeed = 2;
    private Rigidbody rb = null;
    private float life = 100;

    //PathFind
    private int _startNode = 0;
    [SerializeField] private Node[] nodes; //Que tenga todos los nodos
    [SerializeField] private float smoothnessTurn = 1;
    private PathfindController _myPathfindController;
    private int wayPointIncrease = 1;
    private int nextWayPoint = 0;

    //Ammo
    private float _reloadingAmmoTime = 5;
    private const int _maxAmmo = 5;
    private int _ammoLeft = 0;

    //IsTargetNear
    private float _nearDistance = 4f;

    //Walk
    private float _walkTimeBeforeIdle = 5f;
    private float _currentWalkedTime = 0;

    private FSMController<string> _myFSMController;
    private LineOfSight _lineOfSigh = null;
    private INode _initialNode;

    //Steering
    private Pursuit pursuitSteering;

    public event Action OnShoot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        RandomWithException rndWithException = new RandomWithException(0, nodes.Length, _startNode);
        var randomEndNode = rndWithException.Randomize();
        _myPathfindController = new PathfindController(nodes[_startNode], nodes[randomEndNode]);
        _myPathfindController.Execute();
        _startNode = randomEndNode;

        _ammoLeft = _maxAmmo;
        _lineOfSigh = GetComponent<LineOfSight>();

        //Steerings
        _targetRB = _target.GetComponent<Rigidbody>();
        pursuitSteering = new Pursuit(_target.transform, this.transform, _targetRB, 2);

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
        print("shooting");
        rb.velocity = Vector3.zero;
        transform.forward = pursuitSteering.GetDirection();
        OnShoot?.Invoke();
        _ammoLeft -= 1;
    } 

    public void Move() //IMOVE
    {
        _currentWalkedTime += Time.deltaTime;
        var direction = GetNextPosition();
        rb.velocity = direction * walkSpeed;
        transform.forward = direction;
    } 

    public void PursuitTarget() //IATTACK 
    {
        print("persiguiendo");
        rb.velocity = pursuitSteering.GetDirection() * walkSpeed;
        transform.forward = pursuitSteering.GetDirection();
    } 

    public void ReloadAmmo() //IATTACK 
    {
        print("reloading ammo");
        rb.velocity = Vector3.zero;
        transform.forward = pursuitSteering.GetDirection();
        reloadGun.Play();
        StartCoroutine(ReloadingAmmo(_reloadingAmmoTime));
    } 

    public void Respawn() { }

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
        return _lineOfSigh.IsTargetInSight(_target);
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

    private Vector3 GetNextPosition()
    {
        var nextPointPosition = _myPathfindController.AStarResult[nextWayPoint].transform.position;
        Vector3 direction = nextPointPosition - transform.position;

        if (direction.magnitude < smoothnessTurn) 
        {
            if (nextWayPoint < _myPathfindController.AStarResult.Count - 1)
            {
                nextWayPoint += wayPointIncrease;
            }
            else
            {
                nextWayPoint = 0;
                RandomWithException randomWithException = new RandomWithException(0, nodes.Length, _startNode);
                var randomEndNode = randomWithException.Randomize();
                _myPathfindController.EditNodes(nodes[_startNode], nodes[randomEndNode]);
                _myPathfindController.Execute();
                _startNode = randomEndNode;

            }
        }
        return direction.normalized;
    }

    public void DoIdle() //IIdle
    {
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
            life = 100;
            Respawn();
        }
        else
        {
            life -= _damage;
        }
    }
}
