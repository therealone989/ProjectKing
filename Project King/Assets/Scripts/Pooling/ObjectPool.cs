using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField] int prewarmCount = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();


    private void Awake()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            CreateObject();
        }
    }

    GameObject CreateObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            CreateObject();
        }
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
