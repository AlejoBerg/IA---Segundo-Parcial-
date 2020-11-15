using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance { get; private set; }
    List<GameObject> pool = new List<GameObject>();
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialAmount = 5;

    private void Awake()
    {
        Instance = this;
        FillPool();
    }

    void FillPool()
    {
        for (int t=0; t < initialAmount; t++)
        {
            GameObject go = Instantiate(prefab, transform.position, transform.rotation);
            go.SetActive(false);
            pool.Add(go);
        }
    }
    public GameObject Get()
    {
        GameObject ret;
        if (pool.Count > 0 )
        {
            ret = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            GameManager.Instance.cops.Add(ret);
        }
        else
        {
            ret = Instantiate(prefab, transform.position, transform.rotation);
            GameManager.Instance.cops.Add(ret);
        }
        ret.SetActive(true);
        return ret;
    }

     public void Return(GameObject go)
     {
        go.SetActive(false);
        pool.Add(go);
        GameManager.Instance.cops.Remove(go);
     }
}



