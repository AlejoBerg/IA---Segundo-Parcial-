using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Roulette : MonoBehaviour
{    public T Run<T>(Dictionary<T, int> dic)
    {
        float total = 0;
        foreach (var item in dic)
        {
            total += item.Value;
        }
        float random = Random.Range(0, total);

        foreach (var item in dic)
        {
            random -= item.Value;
            if (random < 0)
            {
                return item.Key;
            }
        }
        return default(T);
    }
}
