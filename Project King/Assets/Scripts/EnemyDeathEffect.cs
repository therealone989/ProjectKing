using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    private ParticleSystem ps;
    private ObjectPool myPool;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void Play(ObjectPool pool)
    {
        myPool = pool;
        ps.Play();
        // Wir nutzen Invoke statt Coroutine für weniger Overhead auf Mobile
        Invoke(nameof(ReturnToPool), ps.main.duration + ps.main.startLifetime.constantMax);
    }

    void ReturnToPool()
    {
        myPool.ReturnObject(gameObject);
    }
}
