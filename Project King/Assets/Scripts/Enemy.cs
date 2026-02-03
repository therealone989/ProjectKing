using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform[] WayPoints;
    private Transform endPoint;
    int waypointIndex;

    [Header("Enemy Attributes")]
    [SerializeField] int health = 20;
    public int moveSpeed = 6;
    public System.Action<Enemy> OnDeath;
    bool isInitialized = false;

    [Header("Object Pooling")]
    [SerializeField] private ObjectPool coinPool;
    [SerializeField] private int coinCount = 5;

    [Header("Animation control")]
    [SerializeField] private Animator animator;
    [SerializeField] private float deathDespawnDelay = 1.0f;
    private bool isDying = false;


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
        Transform currentTarget = (waypointIndex < WayPoints.Length) ? WayPoints[waypointIndex] : endPoint;
        Vector3 dir = currentTarget.position - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, currentTarget.position) <= 0.2f)
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
        transform.tag = "Dead";
        OnDeath?.Invoke(this);
        
    
        enabled = false;

      
        SpawnCoins();

    
        if (animator != null)
            animator.SetTrigger("Die");

    
        Destroy(gameObject, deathDespawnDelay);
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
