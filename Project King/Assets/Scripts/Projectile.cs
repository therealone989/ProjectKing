using UnityEngine;

public class Projectile : MonoBehaviour
{
    ObjectPool pool;
    Enemy target;
    int damage;
    public float speed = 10f;
    bool isInitialized = false;

    public void Init(Enemy e, int dmg, ObjectPool pool)
    {
        target = e;
        damage = dmg;
        this.pool = pool;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;
        if (target == null)
        {
            pool.ReturnObject(gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        transform.right = -dir.normalized;
        transform.position += dir.normalized * speed * Time.deltaTime;

        if (dir.magnitude < 0.3f)
        {
            target.takeDamage(damage);
            pool.ReturnObject(gameObject);
        }
    }
}
