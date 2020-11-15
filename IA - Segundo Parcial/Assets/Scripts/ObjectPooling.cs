using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance { get; private set; }
    List<GameObject> pool = new List<GameObject>();
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialAmount = 5;               //Arrancan 5 policias

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
        }
        else
        {
            ret = Instantiate(prefab, transform.position, transform.rotation);
        }
        ret.SetActive(true);
        return ret;
    }

     public void Return(GameObject go)
     {
        go.SetActive(false);
        pool.Add(go);
     }

    // Pool.Instance.Get().transform.position = policemanRespawn.position;    Esto lo tenes que poner donde esta la condicion de que el policia muere 
    
    /*void OnEnable()
    {
        StartCoroutine(ReturnToPoolCoroutine());
    }
                                                                                 Esto se lo asignas al script del policia donde hace respawn. ttl es una variable que significa tiempo de vida, lo puse para la coroutine, pero vos cambia esa variable por lo que quieras.
    IEnumerator ReturnToPoolCoroutine()
    {
        yield return new WaitForSeconds(ttl);
        ObjectPooling.Instance.Return(GameObject);
    }*/

}



