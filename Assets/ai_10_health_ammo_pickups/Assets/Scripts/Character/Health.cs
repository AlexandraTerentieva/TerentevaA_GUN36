using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float lowHealth = 20;

    public float blinkIntensity = 10;
    public float blinkDuration = 0.05f;

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private UIHealthBar healthBar;
    private float blinkTimer;

    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        healthBar = GetComponentInChildren<UIHealthBar>();

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;

            if (hitBox.gameObject != gameObject)
            {
                // Проверяем, существует ли слой Hitbox
                int hitboxLayer = LayerMask.NameToLayer("Hitbox");
                if (hitboxLayer != -1)
                {
                    hitBox.gameObject.layer = hitboxLayer;
                }
                else
                {
                    // Если слоя нет, оставляем текущий
                    Debug.LogWarning("Layer 'Hitbox' not found. Using default layer.");
                }
            }
        }

        OnStart();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthBar != null)
            healthBar.SetHealthBarPercentage(currentHealth / maxHealth);

        OnHeal(amount);
        blinkTimer = blinkDuration;
    }

    public void TakeDamage(float amount, Vector3 direction)
    {
        currentHealth -= amount;

        if (healthBar != null)
            healthBar.SetHealthBarPercentage(currentHealth / maxHealth);

        OnDamage(direction);
        blinkTimer = blinkDuration;

        if (currentHealth <= 0f)
            Die(direction);
    }

    public bool IsDead() => currentHealth <= 0;
    public bool IsLowHealth() => currentHealth < lowHealth;

    private void Die(Vector3 direction) => OnDeath(direction);

    private void Update()
    {
        if (skinnedMeshRenderer == null) return;

        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1.0f;
        skinnedMeshRenderer.material.color = Color.white * intensity;
    }

    protected virtual void OnStart() { }
    protected virtual void OnDeath(Vector3 direction) { }
    protected virtual void OnDamage(Vector3 direction) { }
    protected virtual void OnHeal(float amount) { }
}