using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{

    public Transform shootPoint;
    public GameObject arrowPrefab;
    public int damage = 5;
    public int cooldownMs = 500;

    List<Enemy> enemiesInRange = new();
    int lastAttackTime;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(enemiesInRange.Count == 0) return;
        if (Time.time * 1000 - lastAttackTime < cooldownMs) return;

        Enemy target = enemiesInRange[0];
        Shoot(target);
        lastAttackTime = (int)(Time.time * 1000);

    }

    void Shoot(Enemy target)
    {
        GameObject proj = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);

        // Pfeil wird Initialisiert mit unsere Damage werte und unseren Target - > Projectile Script lässt pfeil zum gegner fliegen
        proj.GetComponent<Projectile>().Init(target, damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && 
            other.TryGetComponent(out Enemy e))
        {
            enemiesInRange.Add(e);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") &&
            other.TryGetComponent(out Enemy e))
        {
            enemiesInRange.Remove(e);
        }
    }
}
