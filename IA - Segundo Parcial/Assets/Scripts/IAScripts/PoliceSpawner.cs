using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    [SerializeField] private ActiveAlarmAndIndicator activatePoolRef;
    private bool canInstantiateIA = false;
    private float timerCounterToSpawn = 10f;
    private float curretTimer = 0;

    private void Start()
    {
        activatePoolRef.OnActivatePool += OnActivatePoolHandler;
    }

    private void Update()
    {
        if (canInstantiateIA)
        {
            if(curretTimer >= timerCounterToSpawn)
            {
                curretTimer = 0;
                ObjectPooling.Instance.Get();
            }
            else
            {
                curretTimer += Time.deltaTime;
            }
        }
    }

    private void OnActivatePoolHandler()
    {
        canInstantiateIA = true;
    }
}
