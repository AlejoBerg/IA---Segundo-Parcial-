using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : INode
{
    public delegate void MyDelegate();
    MyDelegate _nodeActions;

    public ActionNode(MyDelegate initialAction)
    {
        _nodeActions += initialAction;
    }

    public void AddActionToNode(MyDelegate newAction)
    {
        _nodeActions += newAction;
    }

    public void Execute()
    {
        _nodeActions();
    }
}
