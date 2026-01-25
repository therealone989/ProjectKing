using UnityEngine;

public class Projectile : MonoBehaviour
{
    Enemy target;
    int damage;
    public float speed = 10f;

    public void Init(Enemy e, int dmg)
    {
        target = e;
        damage = dmg;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        transform.right = -dir.normalized;
        transform.position += dir.normalized * speed * Time.deltaTime;

        if (dir.magnitude < 0.3f)
        {
            // TAKEDAMAGE ENEMY SCRIPT
            Destroy(gameObject);
        }
    }
}
