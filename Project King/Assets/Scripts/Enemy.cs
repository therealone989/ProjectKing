using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Transform[] WayPoints;
    private Transform endPoint;
    int waypointIndex;

    [Header("Enemy Attributes")]
    [SerializeField] int health, maxHealth = 20;
    public int moveSpeed = 6;
    public float turnSpeed = 10f;
    public System.Action<Enemy> OnDeath;
    bool isInitialized = false;
    public bool IsAlive { get; private set; }

    [Header("Object Pooling")]
    [SerializeField] private ObjectPool coinPool;
    [SerializeField] private ObjectPool healthBarPool;
    [SerializeField] private int coinCount = 5;
    private ObjectPool myPool;

    [Header("Animation control")]
    [SerializeField] private Animator animator;
    [SerializeField] private float deathDespawnDelay = 1.0f;
    private bool isDying = false;

    private FloatingHealthbar healthBar;
    
    private void OnEnable()
    {
        IsAlive = true;
        isDying = false;
        isInitialized = false;
        // Wir machen hier KEIN GetObject für die Healthbar mehr!

        if (animator != null)
            animator.Rebind();
    }
    public void ActivateEnemy(Transform[] path, Transform end)
    {
        // Erst hier initialisieren wir die Logik, die die Pools braucht
        if (healthBarPool != null)
        {
            healthBar = healthBarPool.GetObject().GetComponent<FloatingHealthbar>();
            healthBar.Init(transform);
            health = maxHealth;
            healthBar.UpdateHealthBar(health, maxHealth);
        }
        else{Debug.LogError("HealthBarPool ist null beim Aktivieren!");}

        this.WayPoints = path;
        this.endPoint = end;
        this.health = maxHealth; // Reset health
        this.waypointIndex = 0;
        this.isInitialized = true;
    }
    public void SetPool(ObjectPool pool)
    {
        myPool = pool;
    }
    public void InjectPools(ObjectPool coinPool, ObjectPool healthBarPool)
    {
        this.coinPool = coinPool;
        this.healthBarPool = healthBarPool;
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;
        MoveEnemy();
    }
    private void MoveEnemy()
    {
        // MOOVING STARTED
        Transform currentTarget = (waypointIndex < WayPoints.Length) ? WayPoints[waypointIndex] : endPoint;
        Vector3 dir = currentTarget.position - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.fixedDeltaTime, Space.World);

        // ROTATING STARTED
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.fixedDeltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (Vector3.Distance(transform.position, currentTarget.position) <= 0.2f)
        {
            GetNextWaypoint();
        }
    }
    private void GetNextWaypoint()
    {
        if (waypointIndex >= WayPoints.Length)
        {
            Die();
        }
        waypointIndex++;
    }
    public void takeDamage(int dmg)
    {
        if (!IsAlive) return;
        if (healthBar == null) return;

        health = health - dmg;
        healthBar.UpdateHealthBar(health, maxHealth);
        if(health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (isDying) return;
        isDying = true;

        IsAlive = false;
        isInitialized = false;

        OnDeath?.Invoke(this); // Turrets Informieren
        WaveSpawner.enemiesAlive--;

        if (healthBar != null)
        {
            healthBarPool.ReturnObject(healthBar.gameObject);
            healthBar = null;
        }
        //enabled = false;

        SpawnCoins();
        StartCoroutine(ReturnAfterDelay());
    }
    IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(deathDespawnDelay);

        OnDeath = null;

        if (myPool != null)
            myPool.ReturnObject(gameObject);
        else
            gameObject.SetActive(false); // Fallback
    }
    private void SpawnCoins()
    {
        // Sicherheitscheck: Falls coinPool null ist, hol ihn dir aus der Registry
        if (coinPool == null)
        {
            coinPool = PoolRegistry.Instance.CoinPool;
        }

        if (coinPool == null)
        {
            Debug.LogError("CoinPool konnte auch in der Registry nicht gefunden werden!");
            return;
        }
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coinGO = coinPool.GetObject();
            coinGO.transform.position = transform.position + Vector3.up * 0.5f;
            CoinDrop coin = coinGO.GetComponent<CoinDrop>();
            coin.Init(coinPool);
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
