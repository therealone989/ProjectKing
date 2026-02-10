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

    void FixedUpdate()
    {
        if (!isInitialized) return;
        if (target == null || !target.IsAlive)
        {
            pool.ReturnObject(gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        transform.right = -dir.normalized;
        transform.position += dir.normalized * speed * Time.fixedDeltaTime;

        if ((target.transform.position - transform.position).sqrMagnitude < 0.09f)
        {
            target.takeDamage(damage);
            pool.ReturnObject(gameObject);
        }
    }
}
