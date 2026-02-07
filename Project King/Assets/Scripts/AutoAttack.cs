using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{

    [Header("Object Pooling")]
    [SerializeField] ObjectPool bulletPool;

    [Header("Attributes")]
    public int damage = 5;
    public int cooldownMs = 500;

    [Header("Unity Setup Fields")]
    public Transform shootPoint;
    public GameObject arrowPrefab;
    private readonly List<Enemy> enemiesInRange = new();
    private int lastAttackTime;

    private void FixedUpdate()
    {
      
        enemiesInRange.RemoveAll(e => e == null || !e.IsAlive);

        if (enemiesInRange.Count == 0) return;
        if (Time.time * 1000f - lastAttackTime < cooldownMs) return;

        Enemy target = enemiesInRange[0];
        Shoot(target);
        lastAttackTime = (int)(Time.time * 1000f);
    }

    private void Shoot(Enemy target)
    {
        if (target == null) return;
        GameObject bullet = bulletPool.GetObject();
        bullet.transform.position = shootPoint.position;
        bullet.transform.rotation = shootPoint.rotation;
        bullet.GetComponent<Projectile>().Init(target.GetComponent<Enemy>(), damage, bulletPool);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out Enemy e))
        {
            if (!enemiesInRange.Contains(e))
            {
                e.OnDeath += RemoveEnemy;
                enemiesInRange.Add(e);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out Enemy e))
        {
            RemoveEnemy(e);
        }
    }

    private void RemoveEnemy(Enemy e)
    {
        if (e == null) return;

        e.OnDeath -= RemoveEnemy;
        enemiesInRange.Remove(e);
    }
}
