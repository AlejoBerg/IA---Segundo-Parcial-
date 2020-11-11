using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour, IMove, IAttack
{
    [SerializeField] private GameObject _target = null;

    


    //Ammo
    private float _reloadingAmmoTime = 2;
    private const int _maxAmmo = 5;
    private int _ammoLeft = 0;

    //IsTargetNear
    private float _nearDistance = 2f;

    //Walk
    private float _walkTimeBeforeIdle = 5f;
    private float _currentWalkedTime = 0;

    private FSMController<string> _myFSM;
    private INode _initialNode;
    private LineOfSight _lineOfSigh = null;

    private void Start()
    {
        _ammoLeft = _maxAmmo;

        _lineOfSigh = GetComponent<LineOfSight>();

        //FSM
        IdleState<string> idle = new IdleState<string>();
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

        _myFSM = new FSMController<string>(walk);

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
        _myFSM.OnUpdate();
        _initialNode.Execute();
    }

    public void KillTarget() { } //IATTACK 

    public void Move() //IMOVE
    {
        _currentWalkedTime += Time.deltaTime;

        //Logica de moverse
    } 

    public void PursuitTarget() { } //IATTACK

    public void ReloadAmmo() //IATTACK 
    {
        StartCoroutine(ReloadingAmmo(_reloadingAmmoTime));
    } 

    public void Respawn() { }

    public bool WalkedTime()
    {
        if(_currentWalkedTime > _walkTimeBeforeIdle)
        {
            _currentWalkedTime = 0;
            return true;
        }
        else { return false; }
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

    IEnumerator ReloadingAmmo(float reloadingAmmoTime)
    {
        //Ejecutar animacion
        yield return new WaitForSeconds(reloadingAmmoTime);
        _ammoLeft = _maxAmmo;
    }
}
