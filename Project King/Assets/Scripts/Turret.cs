using UnityEngine;

public class Turret : MonoBehaviour
{

    public Transform target;

    [Header("Object Pooling")]
    [SerializeField] ObjectPool bulletPool;

    [Header("Attributes")]
    public float range = 15f;
    public float fireRate = 1f;
    public int damage = 10;
    private float fireCountdown = 0f;


    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public Transform firePoint;

    private Collider[] hitResults = new Collider[20];
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Animator animator;
    public ParticleSystem shootEffect;
    // Es wird nicht so oft gecheckt - WENIGER RESSOURCEN VERBRAUCHT
    // Distance checks nimmt power
    // 2 mal die sekunde aufgerufen anstatt 60 oder 200
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.1f);

        // Wir holen uns den Pool direkt aus der Registry
        if (PoolRegistry.Instance != null)
        {
            bulletPool = PoolRegistry.Instance.CanonBulletPool;
        }
        else
        {
            Debug.LogError("PoolRegistry nicht in der Szene gefunden!");
        }

    }

    void UpdateTarget()
    {

        // 1. Wenn wir schon ein Ziel haben, prÅEen ob es noch valide ist
        if(target != null)
        {
            Enemy targetEnemy = target.GetComponent<Enemy>();
            // 1. Berechne die quadrierte Distanz (keine Wurzel!)
            Vector3 offset = target.position - transform.position;
            float sqrDistance = offset.sqrMagnitude;
            // Wenn das aktuelle Ziel noch lebt und in Reichweite ist: Behalte es!
            if (targetEnemy != null && targetEnemy.IsAlive && sqrDistance <= range * range)
            {
                return;
            }
        }

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, range, hitResults, enemyLayer);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        for (int i = 0; i < hitCount; i++)
        {
            Enemy e = hitResults[i].GetComponent<Enemy>();
            if (e == null || !e.IsAlive) continue; // Ignoriere tote Gegner im Pool

            float distanceToEnemy = (hitResults[i].transform.position - transform.position).sqrMagnitude; 
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = hitResults[i].gameObject;
            }
        }
        // Wenn ein gegner in range ist und es der n‰herste ist und es noch lebt
        target = (nearestEnemy != null) ? nearestEnemy.transform : null;
    }

    void Update()
    {
        if (target == null)
        return;

        // Target Lock on
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if(fireCountdown <= 0f)
        {
            animator.SetTrigger("Shoot");
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    public void Shoot()
    {
        // Pool/Firepoint-Absicherung (hilft beim Debuggen)
        if (bulletPool == null || firePoint == null) return;

        // Target kann inzwischen weg sein -> absichern
        if (target == null) return;

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy == null || !enemy.IsAlive) return;

        if (shootEffect != null)
            shootEffect.Play();

        GameObject bullet = bulletPool.GetObject();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj == null) return;

        proj.Init(enemy, damage, bulletPool);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
