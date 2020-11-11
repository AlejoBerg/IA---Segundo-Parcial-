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
        aStarResult = myAStar.RunPathfinding(initialNode, SatisfiesCondition, GetNeighbour, GetConnectionCost, GetHeuristic, 1000);
    }

    private bool SatisfiesCondition(Node _current)
    {
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
