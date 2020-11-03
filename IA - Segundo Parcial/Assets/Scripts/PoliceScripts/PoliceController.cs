using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour, IMove, IAttack
{
    private FSMController<string> _myFSM;
    private INode _initialNode;

    private void Start()
    {
        //FSM
        IdleState<string> idle = new IdleState<string>();
        ReloadState<string> reloadAmmo = new ReloadState<string>(this);
        WalkState<string> walk = new WalkState<string>(this);
        PursuitState<string> pursuit = new PursuitState<string>(this);
        KillState<string> kill = new KillState<string>(this);

        _myFSM = new FSMController<string>(idle);

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

        //TREE
        ActionNode respawn = new ActionNode(Respawn);

        QuestionNode haveAmmo = new QuestionNode(CheckAmmoLeft, kill, reloadAmmo);
        QuestionNode isTargetNear = new QuestionNode(IsTargetNear, haveAmmo, pursuit);
        QuestionNode amIExhausted = new QuestionNode(WalkedALot, idle, walk);
        QuestionNode isInSight = new QuestionNode(IsTargetInSight, isTargetNear, amIExhausted);
    }

    private void Update()
    {
        _myFSM.OnUpdate();
    }

    public void KillTarget() { } //IATTACK 

    public void Move() { } //IMOVE

    public void PursuitTarget() { } //IATTACK

    public void ReloadAmmo() { } //IATTACK

    public void Respawn() { }

    public bool WalkedALot()
    {
        return false;
    }

    public bool IsTargetInSight() 
    {
        return false;
    }

    public bool CheckAmmoLeft()
    {
        return false;
    }

    public bool IsTargetNear()
    {
        return false;
    }
}
