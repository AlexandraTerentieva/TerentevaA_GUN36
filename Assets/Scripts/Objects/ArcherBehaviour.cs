using UnityEngine;

/// <summary>
/// Лучник. Бегает к врагам, стреляет, перезаряжается.
/// </summary>
public class ArcherBehaviour : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ (заполняются в инспекторе)
    // ============================================================

    [Header("Настройки")]
    public float moveSpeed = 3f;          // Скорость бега к врагу
    public float attackRange = 5f;        // Дистанция, с которой начинает стрелять
    public float fireRate = 1f;           // Задержка между выстрелами
    public int damage = 5;                // Урон за один выстрел
    public GameObject arrowPrefab;        // Префаб стрелы (перетащить в инспекторе)

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private Transform target;             // Текущая цель (ближайший враг)
    private float cooldown;               // Оставшееся время перезарядки

    // ============================================================
    //  ЛОГИКА ПОВЕДЕНИЯ (вызывается каждый кадр)
    // ============================================================

    void Update()
    {
        // Ищем ближайшего врага
        FindTarget();

        // Если врагов нет — стоим на месте
        if (target == null) return;

        // Считаем расстояние до цели
        float distance = Vector3.Distance(transform.position, target.position);

        // Если враг далеко — бежим к нему
        if (distance > attackRange)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            // Если враг в радиусе — стреляем с перезарядкой
            cooldown -= Time.deltaTime;
            if (cooldown <= 0f)
            {
                Shoot();
                cooldown = fireRate;
            }
        }
    }

    // ============================================================
    //  ПОИСК БЛИЖАЙШЕГО ВРАГА
    // ============================================================

    /// <summary>
    /// Находит ближайшего врага с тегом "Enemy".
    /// </summary>
    void FindTarget()
    {
        // Находим всех врагов на сцене
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float closestDist = Mathf.Infinity;
        target = null;

        // Перебираем всех врагов, ищем самого близкого
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                target = enemy.transform;
            }
        }
    }

    // ============================================================
    //  ВЫСТРЕЛ
    // ============================================================

    /// <summary>
    /// Создаёт стрелу и отправляет её в цель.
    /// </summary>
    void Shoot()
    {
        if (arrowPrefab == null || target == null) return;

        // Создаём стрелу в позиции лучника
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        // Передаём стреле цель и урон
        ArrowProjectile proj = arrow.GetComponent<ArrowProjectile>();
        if (proj != null)
        {
            proj.Initialize(target, damage);
        }
    }
}