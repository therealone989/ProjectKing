using UnityEngine;
using System.Collections.Generic;

public class EnemyPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public EnemyType type; // Schluessel
        public ObjectPool pool; // Value
    }

    [SerializeField] private Entry[] pools;

    private Dictionary<EnemyType, ObjectPool> dict;
    

    private void Awake()
    {
        dict = new Dictionary<EnemyType, ObjectPool>();
        foreach (var e in pools)
            dict[e.type] = e.pool;
    }

    public ObjectPool GetPool(EnemyType type)
    {
        return dict[type];
    }

    public GameObject GetEnemy(EnemyType type)
    {
        return dict[type].GetObject();
    }
}
