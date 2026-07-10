using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;
    public UIHealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetHealthBarPercentage(1f);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthBar != null)
            healthBar.SetHealthBarPercentage(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
        }
    }
}