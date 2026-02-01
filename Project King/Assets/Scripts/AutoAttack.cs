using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject arrowPrefab;
    public int damage = 5;
    public int cooldownMs = 500;

    private readonly List<Enemy> enemiesInRange = new();
    private int lastAttackTime;

    private void FixedUpdate()
    {
      
        enemiesInRange.RemoveAll(e => e == null);

        if (enemiesInRange.Count == 0) return;
        if (Time.time * 1000f - lastAttackTime < cooldownMs) return;

        Enemy target = enemiesInRange[0];
        Shoot(target);
        lastAttackTime = (int)(Time.time * 1000f);
    }

    private void Shoot(Enemy target)
    {
        if (target == null) return;

        GameObject proj = (GameObject)Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(target, damage);
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
