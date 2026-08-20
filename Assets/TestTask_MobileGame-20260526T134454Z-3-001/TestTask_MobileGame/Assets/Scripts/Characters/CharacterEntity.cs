using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private bool isDead = false;
    private float lastAttackTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"✅ {name}: Здоровье = {currentHealth}");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"💥 {name} получил {damage} урона. Осталось: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"💀 {name} УМЕР!");

        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.SetTrigger("Death");

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Destroy(gameObject, 3f);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Attack(CharacterEntity target)
    {
        if (isDead) return;
        if (target == null) return;
        if (target.IsDead()) return;

        if (Time.time - lastAttackTime < attackCooldown) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange) return;

        lastAttackTime = Time.time;
        target.TakeDamage(attackDamage);
        Debug.Log($"⚔️ {name} атаковал {target.name} на {attackDamage} урона!");
    }
}