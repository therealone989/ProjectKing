using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image[] healthBars;
    float health;
    float maxHealth = 100f;

    void Start()
    {
        health = maxHealth;
        HealthBarFiller();
    }

    public void InputPlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerDamage(7);
        }
    }

    void HealthBarFiller()
    {
        // Wir berechnen den Wert pro Balken dynamisch basierend auf der Array-Größe
        float healthPerBar = maxHealth / healthBars.Length;

        for (int i = 0; i < healthBars.Length; i++)
        {
            // Wir berechnen den Startpunkt dieses spezifischen Balkens (z.B. Bar 0 startet bei 0, Bar 1 bei 10...)
            float barThreshold = i * healthPerBar;

            // Wie viel Leben "gehört" in diesen Balken?
            // Wir nehmen das aktuelle Leben, ziehen den Startpunkt ab und teilen durch die Kapazität des Balkens
            float fillValue = (health - barThreshold) / healthPerBar;

            // Der Wert muss zwischen 0 (leer) und 1 (voll) liegen
            healthBars[i].fillAmount = Mathf.Clamp01(fillValue);
        }
    }
    public void PlayerDamage(float damagePoints)
    {
        health -= damagePoints;
        health = Mathf.Clamp(health, 0, maxHealth); // Verhindert Werte unter 0 oder über Max
        HealthBarFiller();
    }
}
