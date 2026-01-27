using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Transform[] WayPoints;
    private Transform endPoint;
    int waypointIndex;
    int moveSpeed = 6;
    int rotationSpeed = 230;
    float reachDistance = 0.15f;

    int health = 20;
    public System.Action<Enemy> OnDeath;
    bool isInitialized = false;

    public void Init(Transform[] path, Transform end)
    {
        waypointIndex = 0;
        this.WayPoints = path;
        this.endPoint = end;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        Transform currentTarget = (waypointIndex < WayPoints.Length) ? WayPoints[waypointIndex] : endPoint;
        Vector3 dir = currentTarget.transform.position - transform.position;
        //dir.y = 0f;
        float dist = dir.magnitude;

        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
        transform.position += move;

        if (dist <= reachDistance)
        {
            if(waypointIndex < WayPoints.Length )
            {
                waypointIndex++;
            } else
            {
                Destroy(gameObject);
            }
            
        }

    }

    public void takeDamage(int dmg)
    {
        health = health - dmg;
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
