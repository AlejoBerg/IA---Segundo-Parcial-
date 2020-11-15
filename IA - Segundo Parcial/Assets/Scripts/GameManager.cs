using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static int score = 0;

    public static GameManager Instance { get; private set; }

    public List<GameObject> bandides = new List<GameObject>();
    public List<GameObject> cops = new List<GameObject>();

    public static int Score => score;


    private void Awake()
    {
        Instance = this;
    }


    public static void AddPoints(int newPoints)
    {
        score += newPoints;  
    }
}