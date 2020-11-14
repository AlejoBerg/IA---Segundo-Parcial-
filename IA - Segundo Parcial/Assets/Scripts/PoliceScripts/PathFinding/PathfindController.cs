using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindController
{
    private Node initialNode = null;
    private Node endNode = null;
    PathfindAStar<Node> myAStar = new PathfindAStar<Node>();
    private List<Node> aStarResult = new List<Node>();

    public List<Node> AStarResult { get => aStarResult;}

    public PathfindController(Node _initialNode, Node _endNode)
    {
        initialNode = _initialNode;
        endNode = _endNode;
    }

    public void Execute()
    {
        aStarResult = myAStar.RunPathfinding(initialNode, SatisfiesCondition, GetNeighbour, GetConnectionCost, GetHeuristic, 80);
        Debug.Log("aStarResult.Count = " + aStarResult.Count);
    }

    public void EditNodes(Node _newInitialNode, Node _newEndNode) 
    {
        initialNode = _newInitialNode;
        endNode = _newEndNode;
        Debug.Log($"Los nodos editados son: InitialNode = {initialNode} y endNode = {endNode}");
    }

    private bool SatisfiesCondition(Node _current)
    {
        Debug.Log($"Satisfies condition CurrentNode = {_current} y endNode = {endNode}");
        return _current == endNode;
    }

    private List<Node> GetNeighbour(Node _current) //Retorna vecino en base a current node
    {
        var list = new List<Node>();

        for (int i = 0; i < _current.NeighbourNodes.Count; i++)
        {
            list.Add(_current.NeighbourNodes[i]);
        }
        return list;
    }

    private float GetConnectionCost(Node parentNode, Node childNode) 
    {
        return Vector3.Distance(parentNode.transform.position, childNode.transform.position);
    }

    private float GetHeuristic(Node current) 
    {
        return Vector3.Distance(current.transform.position, endNode.transform.position);
    }
}
