using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{

    [Header("Object Pooling")]
    [SerializeField] ObjectPool bulletPool;

    [Header("Attributes")]
    public int damage = 5;
    public int cooldownMs = 500;
    public float attackRange = 5f;
    public LayerMask enemyLayer;

    [Header("Unity Setup Fields")]
    public Transform shootPoint;
    public GameObject arrowPrefab;

    private Collider[] hitResults = new Collider[10];
    private Enemy currentTarget;
    private float lastAttackTime;


    private void FixedUpdate()
    {
        if(Time.frameCount % 5 == 0)
        {
            FindTarget();
        }

        if(currentTarget != null && currentTarget.IsAlive)
        {
            float distSqr = (currentTarget.transform.position - transform.position).sqrMagnitude;
            if(distSqr > attackRange * attackRange)
            {
                currentTarget = null;
                return;
            }

            if(Time.time * 1000f - lastAttackTime >= cooldownMs)
            {
                Shoot(currentTarget);
                lastAttackTime = Time.time * 1000f;
            }

        }
    }

    void FindTarget()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRange, hitResults,enemyLayer);

        if(hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                Enemy e = hitResults[i].GetComponent<Enemy>();
                if(e != null && e.IsAlive)
                {
                    Debug.Log(e);
                    currentTarget = e;
                    return;
                }
            }
        }
        else { currentTarget = null; }
    }

    private void Shoot(Enemy target)
    {
        if (target == null) return;
        GameObject bullet = bulletPool.GetObject();
        bullet.transform.position = shootPoint.position;
        bullet.transform.rotation = shootPoint.rotation;
        bullet.GetComponent<Projectile>().Init(target.GetComponent<Enemy>(), damage, bulletPool);
    }

    /*
    private void RemoveEnemy(Enemy e)
    {
        if (e == null) return;

        e.OnDeath -= RemoveEnemy;
        enemiesInRange.Remove(e);
    }
    */
}
