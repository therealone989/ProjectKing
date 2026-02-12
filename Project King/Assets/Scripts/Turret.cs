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
    private GameObject bullet = null;


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

        // 1. Wenn wir schon ein Ziel haben, prüfen ob es noch valide ist
        if(target != null)
        {
            Enemy targetEnemy = target.GetComponent<Enemy>();
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Wenn das aktuelle Ziel noch lebt und in Reichweite ist: Behalte es!
            if (targetEnemy != null && targetEnemy.IsAlive && distanceToTarget <= range)
            {
                return;
            }
        }

        // 2. Nur wenn wir kein Ziel haben (oder das alte weg ist), suchen wir ein neues Ziel.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e == null || !e.IsAlive) continue; // Ignoriere tote Gegner im Pool

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        // Wenn ein gegner in range ist und es der näherste ist und es noch lebt
        if(nearestEnemy != null && shortestDistance <= range)
            target = nearestEnemy.transform;
        // Wenn der gegner null ist
        else 
            target = null;
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
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        bullet = bulletPool.GetObject();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.GetComponent<Projectile>().Init(target.GetComponent<Enemy>(), damage, bulletPool);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
