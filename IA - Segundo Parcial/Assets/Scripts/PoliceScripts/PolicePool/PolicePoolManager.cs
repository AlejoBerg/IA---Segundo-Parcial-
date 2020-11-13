using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicePoolManager : MonoBehaviour
{
    [SerializeField] private float timeToSpawn = 5;
    [SerializeField] private Transform spawnPoint = null;
    private float currentTime;

    void Update()
    {
        if(currentTime > timeToSpawn)
        {
            currentTime = 0;
            GameObject obj = ObjectPooler.Instance.GetPooledObject();
            if (obj == null) return;

            obj.transform.position = spawnPoint.position;
            obj.transform.rotation = spawnPoint.rotation;
            obj.SetActive(true);
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
}
