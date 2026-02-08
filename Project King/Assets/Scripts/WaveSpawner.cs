using System.Collections;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Transform startPoint;
    
    [SerializeField] private Path path;

    [Header("Waves")]
    public Wave[] waves;
    public static int enemiesAlive = 0;
    public float timeBetweenWaves = 5f;
    private float countdown = 2f; // Sekunden bist die erste wave gespawnt ist
    private int waveIndex = 0;

    [Header("Pooling")]
    [SerializeField] private EnemyPoolManager enemyPoolManager;
    [SerializeField] private ObjectPool coinPool;
    [SerializeField] private ObjectPool healthBarPool;

    private bool isSpawning = false;

    private void Update()
    {
        // Wenn keine Gegner mehr am Leben dann keine Wave mehr - return
        if( enemiesAlive > 0 || isSpawning) { return; }

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            isSpawning = true;
            countdown = timeBetweenWaves;
            return;
        }
        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        Debug.Log(waveIndex);
        Wave wave = waves[waveIndex];
        for (int i= 0; i< wave.count; i++)
        {
            SpawnEnemy(wave.enemyType);
            yield return new WaitForSeconds(1f / wave.rate);
        }
        waveIndex++;
        isSpawning = false;

        if (waveIndex == waves.Length)
        {
            // End Level ? Next Scene? TO BE CONTINUED DAMM DAMM 
            Debug.Log("LEVEL BEENDET lol");
            this.enabled = false;
        }
    }

    void SpawnEnemy(EnemyType type)
    {
        ObjectPool pool = enemyPoolManager.GetPool(type);
        GameObject enemyGO = pool.GetObject();
        Enemy enemy = enemyGO.GetComponent<Enemy>();

        // 1. Abhängigkeiten injizieren
        enemy.InjectPools(coinPool, healthBarPool);
        enemy.SetPool(pool);

        // 2. Position setzen
        enemyGO.transform.position = startPoint.position;

        // 3. Jetzt erst die Logik starten, die die Healthbar braucht!
        enemy.ActivateEnemy(path.Waypoints, path.endPoint);

        WaveSpawner.enemiesAlive++;
    }
}
