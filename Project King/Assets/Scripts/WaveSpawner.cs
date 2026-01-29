using System.Collections;
using UnityEngine;
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

    private void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
            Debug.Log("COUNTDOWN AFTER RESET" + countdown);
        }
        countdown -= Time.deltaTime;
        Debug.Log(countdown);
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
        Enemy enemy = Instantiate(
            enemyPrefab,
            startPoint.position,
            startPoint.rotation).GetComponent<Enemy>();
        enemy.Init(path.Waypoints, path.endPoint);
    }
}
