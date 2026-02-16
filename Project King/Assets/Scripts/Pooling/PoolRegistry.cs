using UnityEngine;

public class PoolRegistry : MonoBehaviour
{
    // Das Singleton: Ermöglicht Zugriff von überall mit PoolRegistry.Instance
    public static PoolRegistry Instance { get; private set; }

    [Header("Spieler und Gegner Pools")]
    public ObjectPool ArrowPool;
    public ObjectPool CanonBulletPool;
    public ObjectPool CoinPool;
    public ObjectPool Enemy_SlimePool;
    public ObjectPool Enemy_HealthbarPool;

    [Header("Effect Pools")]
    public ObjectPool DeathEffectPool;
    public ObjectPool CanonBulletEffectPool;
    private void Awake()
    {
        // Singleton Logik: Es darf nur eine Registry geben
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Falls der GameManager über Szenen hinweg bestehen bleiben soll:
        DontDestroyOnLoad(gameObject); 
    }
}
