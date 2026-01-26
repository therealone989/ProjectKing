using UnityEngine;

public class Enemy : MonoBehaviour
{
    int health = 20;

    void Start()
    {
        
    }

    void Update()
    {
        
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
        Destroy(gameObject);
    }
}
