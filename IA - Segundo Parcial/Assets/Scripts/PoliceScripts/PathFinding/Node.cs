﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]private List<Node> neighbourNodes;

    public List<Node> NeighbourNodes { get => neighbourNodes;}
}
