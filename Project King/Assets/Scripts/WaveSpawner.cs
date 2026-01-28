using System.Collections;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public int count = 10;
    public float spawnInterval = 0.5f;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Path path;

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private bool autoStart = true;

    private int waveIndex = 0;
    private int aliveEnemies = 0;

    private bool isSpawning = false;

    private void Start()
    {
        if (autoStart)
            StartNextWave();
    }

    public void StartNextWave()
    {
        if (isSpawning) return;                 // schützt vor Doppelstart
        if (waveIndex >= waves.Length) return;  // keine Waves mehr

        StartCoroutine(SpawnWave(waves[waveIndex]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        // Gegner spawnen
        for (int i = 0; i < wave.count; i++)
        {
            SpawnOneEnemy();
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // warten bis alle tot / am Ziel sind
        while (aliveEnemies > 0)
            yield return null;

        isSpawning = false;
        waveIndex++;

        // Pause bis nächste Wave
        if (waveIndex < waves.Length)
            yield return new WaitForSeconds(timeBetweenWaves);

        // Auto-Start nächste Wave
        if (autoStart)
            StartNextWave();
    }

    private void SpawnOneEnemy()
    {
        Enemy e = Instantiate(enemyPrefab, startPoint.position, Quaternion.identity);
        e.Init(path.Waypoints, path.endPoint);

        aliveEnemies++;
        e.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(Enemy e)
    {
        aliveEnemies--;
        e.OnDeath -= HandleEnemyDeath;
    }
}
