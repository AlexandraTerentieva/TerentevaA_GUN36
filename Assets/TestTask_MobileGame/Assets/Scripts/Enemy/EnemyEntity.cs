using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Характеристики и методы врага.
/// Отдельный скрипт для врага (по ТЗ).
/// </summary>
public class EnemyEntity : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Здоровье врага")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("Атака врага")]
    public float attackDamage = 5f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private bool isDead = false;
    private float lastAttackTime = 0f;
    private Renderer enemyRenderer;
    private Color originalColor;

    // ============================================================
    //  СТАРТ
    // ============================================================

    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
        Debug.Log($"✅ Враг {name}: Здоровье = {currentHealth}");
    }

    // ============================================================
    //  ПОЛУЧЕНИЕ УРОНА
    // ============================================================

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"💥 Враг {name} получил {damage} урона. Осталось: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    // ============================================================
    //  СМЕРТЬ
    // ============================================================

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"💀 Враг {name} УМЕР!");

        StartCoroutine(DeathAnimation());
    }

    // ============================================================
    //  АНИМАЦИЯ СМЕРТИ (ЗЕЛЕНЕЕТ И ИСЧЕЗАЕТ)
    // ============================================================

    private IEnumerator DeathAnimation()
    {
        float duration = 1.0f;
        float elapsed = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Плавно меняем цвет на ЗЕЛЁНЫЙ
            if (enemyRenderer != null)
            {
                Color targetColor = Color.Lerp(originalColor, Color.green, progress);
                enemyRenderer.material.color = targetColor;
            }

            // Сжимаем врага
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            // Эффект "удара"
            float bobOffset = Mathf.Sin(progress * Mathf.PI) * 0.2f;
            transform.position += Vector3.up * bobOffset * Time.deltaTime;

            yield return null;
        }

        // Завершение
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.green;
        }
        transform.localScale = Vector3.zero;

        Destroy(gameObject);
    }

    // ============================================================
    //  ПРОВЕРКА СМЕРТИ
    // ============================================================

    public bool IsDead()
    {
        return isDead;
    }

    // ============================================================
    //  АТАКА
    // ============================================================

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
        Debug.Log($"⚔️ Враг {name} атаковал {target.name} на {attackDamage} урона!");
    }
}