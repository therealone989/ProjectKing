using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Transform[] WayPoints;
    private Transform endPoint;
    int waypointIndex;

    [Header("Enemy Attributes")]
    [SerializeField] int health = 20;
    public int moveSpeed = 6;
    public float turnSpeed = 10f;
    public System.Action<Enemy> OnDeath;
    bool isInitialized = false;
    public bool IsAlive { get; private set; }

    [Header("Object Pooling")]
    [SerializeField] private ObjectPool coinPool;
    [SerializeField] private int coinCount = 5;
    private ObjectPool myPool;

    [Header("Animation control")]
    [SerializeField] private Animator animator;
    [SerializeField] private float deathDespawnDelay = 1.0f;
    private bool isDying = false;

    private void OnEnable()
    {
        IsAlive = true; // Für autoattack.cs damit er nicht in die leere schießt
        isDying = false;
        isInitialized = false;
        enabled = true;
        health = 20;
        waypointIndex = 0;

        if (animator != null)
            animator.Rebind();
    }

    public void SetPool(ObjectPool pool)
    {
        myPool = pool;
    }

    public void Init(Transform[] path, Transform end)
    {
        waypointIndex = 0;
        this.WayPoints = path;
        this.endPoint = end;
        isInitialized = true;
    }

    private void Start()
    {
        coinPool = GameObject.Find("CoinPool").GetComponent<ObjectPool>();
    }

    private void Update()
    {
        if (!isInitialized) return;

        // MOOVING STARTED
        Transform currentTarget = (waypointIndex < WayPoints.Length) ? WayPoints[waypointIndex] : endPoint;
        Vector3 dir = currentTarget.position - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);

        // ROTATING STARTED
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
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
        health = health - dmg;
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        OnDeath?.Invoke(this); // Turrets Informieren
        WaveSpawner.enemiesAlive--;

        IsAlive = false;
        isInitialized = false;

        //enabled = false;

        SpawnCoins();
        StartCoroutine(ReturnAfterDelay());
    }

    IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(deathDespawnDelay);

        OnDeath = null;
        myPool.ReturnObject(gameObject);
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coinGO = coinPool.GetObject();
            coinGO.transform.position = transform.position + Vector3.up * 0.5f;
            CoinDrop coin = coinGO.GetComponent<CoinDrop>();
            coin.Init(coinPool);
        }
    }

}
