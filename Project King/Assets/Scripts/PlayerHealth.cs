using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public Image[] healthBars;

    float health, maxHealth = 100f;
    float lerpSpeed;
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarFiller();
    }

    public void InputPlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerDamage(10);
        }
    }

    bool DisplayHealthBars(float _health, int pointNumber)
    {
        return ((pointNumber * 11) >= _health);
    }

    void HealthBarFiller()
    {
        for (int i = 0; i < healthBars.Length; i++)
        {
            healthBars[i].enabled = !DisplayHealthBars(health, i);
        }
    }
    public void PlayerDamage(float damagePoints)
    {
        if(health > 0)
        {
            Debug.Log(health);
            health -= damagePoints;
        }
    }
}
