using System.Collections;
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
    public Transform visualModel;

    [Header("Juice Settings")]
    public float punchScale = 1.2f; // (1.2 = 20% größer)
    public float punchDuration = 0.1f; // Wie schnell es zurückgeht
    public float rotationSpeed = 15f;

    private Collider[] hitResults = new Collider[10];
    private Enemy currentTarget;
    private float lastAttackTime;
    private Vector3 originalScale;

    private void Awake()
    {
        if (visualModel != null) originalScale = visualModel.localScale;
        else originalScale = transform.localScale;
    }

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

            // 2. Rotation zum Gegner (Überschreibt die Bewegungs-Rotation)
            RotateTowardsTarget();

            if (Time.time * 1000f - lastAttackTime >= cooldownMs)
            {
                Shoot(currentTarget);
                lastAttackTime = Time.time * 1000f;
            }

        }
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = currentTarget.transform.position - transform.position;
        dir.y = 0; // Damit der Spieler nicht nach oben/unten kippt
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            // Wir nutzen Slerp für eine weiche Drehung
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void FindTarget()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRange, hitResults,enemyLayer);

        if(hitCount > 0)
        {
            float shortestDistance = Mathf.Infinity;
            Enemy nearestEnemy = null;

            for (int i = 0; i < hitCount; i++)
            {
                Enemy e = hitResults[i].GetComponent<Enemy>();
                if(e != null && e.IsAlive)
                {
                    // Wir nehmen die Distanz zum Spieler
                    float distanceToEnemy = Vector3.Distance(transform.position, e.transform.position);
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = e;
                    }
                }
            }
            currentTarget = nearestEnemy;
        }
        else { currentTarget = null; }
    }

    private void Shoot(Enemy target)
    {
        if (target == null) return;

        // Effekt: Skalierung triggern
        StopAllCoroutines(); // Falls wir sehr schnell schießen
        StartCoroutine(PunchEffect());

        GameObject bullet = bulletPool.GetObject();
        bullet.transform.position = shootPoint.position;
        bullet.transform.rotation = shootPoint.rotation;
        bullet.GetComponent<Projectile>().Init(target.GetComponent<Enemy>(), damage, bulletPool);
    }

    IEnumerator PunchEffect()
    {
        Transform targetTransform = visualModel != null ? visualModel : transform;

        // Kurz groß werden
        targetTransform.localScale = originalScale * punchScale;

        // Ein kurzes Frame warten oder direkt weich zurückskalieren
        float elapsed = 0;
        while (elapsed < punchDuration)
        {
            elapsed += Time.deltaTime;
            targetTransform.localScale = Vector3.Lerp(originalScale * punchScale, originalScale, elapsed / punchDuration);
            yield return null;
        }

        targetTransform.localScale = originalScale;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
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
