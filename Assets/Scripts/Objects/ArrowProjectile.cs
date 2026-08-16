using UnityEngine;
using Netologia.TowerDefence;

/// <summary>
/// Снаряд лучника. Летит в цель, наносит урон, исчезает.
/// </summary>
public class ArrowProjectile : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ (заполняются в инспекторе)
    // ============================================================

    public float speed = 10f;          // Скорость полёта стрелы

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private Transform target;          // Цель, в которую летим
    private int damage;                // Урон, который нанесём при попадании

    // ============================================================
    //  МЕТОДЫ
    // ============================================================

    /// <summary>
    /// Задаёт цель и урон для стрелы.
    /// Вызывается из ArcherBehaviour при выстреле.
    /// </summary>
    public void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    // ============================================================
    //  ЛОГИКА ПОЛЁТА (вызывается каждый кадр)
    // ============================================================

    void Update()
    {
        // Если цель умерла или исчезла — удаляем стрелу
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Летим в сторону цели
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Если долетели до цели — наносим урон
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            HitTarget();
        }
    }

    // ============================================================
    //  НАНЕСЕНИЕ УРОНА
    // ============================================================

    /// <summary>
    /// Наносит урон цели и уничтожает стрелу.
    /// </summary>
    void HitTarget()
    {
        // Пытаемся получить компонент Unit у цели
        Unit unit = target.GetComponent<Unit>();

        if (unit != null)
        {
            // Отнимаем здоровье
            unit.CurrentHealth -= damage;

            // Если здоровье <= 0 — враг умирает
            if (unit.CurrentHealth <= 0)
            {
                Destroy(unit.gameObject);
            }
        }

        // Уничтожаем стрелу
        Destroy(gameObject);
    }
}