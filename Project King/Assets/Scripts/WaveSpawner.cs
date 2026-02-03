using System.Collections;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Transform startPoint;
    [SerializeField] public Transform enemyPrefab;
    [SerializeField] private Path path;

    [Header("Waves")]
    public float timeBetweenWaves = 5f;
    private float countdown = 2f; // Sekunden bist die erste wave gespawnt ist
    private int waveIndex = 0;

    [Header("Pooling")]
    [SerializeField] private ObjectPool enemyPool;


    private void Start()
    {
        enemyPool = GameObject.Find("EnemyPool").GetComponent<ObjectPool>();
    }


    private void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        waveIndex++;
        for (int i= 0; i< waveIndex; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SpawnEnemy()
    {
        GameObject enemyGO = enemyPool.GetObject();
        enemyGO.transform.position = startPoint.position;
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.Init(path.Waypoints, path.endPoint);
    }
}
