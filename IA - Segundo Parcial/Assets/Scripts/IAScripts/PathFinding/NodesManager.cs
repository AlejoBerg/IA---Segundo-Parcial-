using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesManager : MonoBehaviour
{
    public static NodesManager Instance { get; private set; }
    [SerializeField] private Node[] nodes;

    private void Awake()
    {
        //print("me creo");
        Instance = this;
    }

    public Node[] GetNodes()
    {
        return nodes;
    }
}
