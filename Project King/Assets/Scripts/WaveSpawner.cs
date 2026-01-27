using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] Transform StartPoint;
    [SerializeField] Enemy enemyPrefab;
    public Path path;

    private void Awake()
    {
        Enemy enemy = Instantiate(enemyPrefab, StartPoint.position, Quaternion.identity);
        enemy.Init(path.Waypoints, path.endPoint);
    }

}
