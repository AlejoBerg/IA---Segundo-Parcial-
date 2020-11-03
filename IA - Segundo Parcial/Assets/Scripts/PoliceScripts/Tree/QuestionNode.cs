using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNode : INode
{
    public delegate bool MyDelegate();
    MyDelegate _question;
    INode _trueNode;
    INode _falseNode;

    public QuestionNode(MyDelegate question, INode trueNode, INode falseNode)
    {
        _question = question;
        _trueNode = trueNode;
        _falseNode = falseNode;
    }

    public void Execute()
    {
        if (_question())
        {
            _trueNode.Execute();
        }
        else
        {
            _falseNode.Execute();
        }
    }
}
