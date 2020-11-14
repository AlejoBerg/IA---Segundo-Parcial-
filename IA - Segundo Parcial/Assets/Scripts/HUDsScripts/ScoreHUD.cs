using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHUD : MonoBehaviour
{
    [SerializeField] private Text moneyPicked;
    
    void Update()
    {
        moneyPicked.text = ("$" + " " + (GameManager.Score).ToString());
    }
}
