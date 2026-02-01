using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Transform[] WayPoints;
    private Transform endPoint;
    int waypointIndex;
    int moveSpeed = 6;
    int rotationSpeed = 230;

    [SerializeField] int health = 20;
    public System.Action<Enemy> OnDeath;
    bool isInitialized = false;

    [SerializeField] private Animator animator;

    [Header("Loot COIN")]
    [SerializeField] private Coin coinPrefab; 
    [SerializeField] private int coinCount = 5;
    [SerializeField] private float spawnHeight = 0.8f;

    [SerializeField] private float scatterRadius = 1.2f; 
    [SerializeField] private float arcHeight = 1.0f;    
    [SerializeField] private float flightTime = 0.22f;   

    [SerializeField] private LayerMask groundMask = ~0;  
    [SerializeField] private float deathDespawnDelay = 1.0f;
    private bool isDying = false;


    public void Init(Transform[] path, Transform end)
    {
        waypointIndex = 0;
        this.WayPoints = path;
        this.endPoint = end;
        isInitialized = true;
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
        if (coinPrefab == null || coinCount <= 0) return;

        Vector3 origin = transform.position + Vector3.up * spawnHeight;

        for (int i = 0; i < coinCount; i++)
        {
            Vector2 r = Random.insideUnitCircle.normalized;
            Vector3 offset = new Vector3(r.x, 0f, r.y) * Random.Range(scatterRadius * 0.6f, scatterRadius);

            Vector3 target = origin + offset;

            Coin c = Instantiate(coinPrefab, origin, Random.rotation);

            // Bodenpunkt finden
            if (Physics.Raycast(target + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, groundMask))
            {
                float halfHeight = c.GetHalfHeight();         
                float extra = 0.01f;                         
                target = hit.point + Vector3.up * (halfHeight + extra);
            }

            c.Launch(target, flightTime, arcHeight * Random.Range(0.85f, 1.15f));
        }
    }

}
