using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWithException
{
    int _min;
    int _max;
    int _exception; 

    public RandomWithException (int min, int max, int exception)
    {
        _min = min;
        _max = max;
        _exception = exception;
    }
    
    public int Randomize()
    {
        int result = Random.Range(_min, _max - 1);
        if (result == _exception) result += 1;
        return result;
    }
}
