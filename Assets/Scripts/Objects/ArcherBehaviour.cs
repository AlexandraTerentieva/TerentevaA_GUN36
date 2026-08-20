using UnityEngine;
using Netologia.Systems;

/// <summary>
/// Лучник. Бегает к врагам, стреляет, перезаряжается.
/// </summary>
public class ArcherBehaviour : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ (заполняются в инспекторе)
    // ============================================================

    [Header("Настройки")]
    [SerializeField] private float moveSpeed = 3f;          // Скорость бега к врагу
    [SerializeField] private float attackRange = 5f;        // Дистанция, с которой начинает стрелять
    [SerializeField] private float fireRate = 1f;           // Задержка между выстрелами
    [SerializeField] private int damage = 5;                // Урон за один выстрел
    [SerializeField] private GameObject arrowPrefab;        // Префаб стрелы

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private Transform target;             // Текущая цель (ближайший враг)
    private float cooldown;               // Оставшееся время перезарядки
    private UnitSystem _unitSystem;       // Ссылка на систему юнитов

    // ============================================================
    //  СТАРТ
    // ============================================================

    void Start()
    {
        // Находим UnitSystem на сцене
        _unitSystem = FindObjectOfType<UnitSystem>();
        if (_unitSystem == null)
        {
            Debug.LogError("ArcherBehaviour: UnitSystem не найден!");
        }
    }

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
    //  ПОИСК БЛИЖАЙШЕГО ВРАГА (через UnitSystem)
    // ============================================================

    /// <summary>
    /// Находит ближайшего врага через UnitSystem.
    /// Оптимизировано: нет FindGameObjectsWithTag.
    /// </summary>
    private void FindTarget()
    {
        if (_unitSystem == null) return;

        float closestDist = Mathf.Infinity;
        target = null;

        // Перебираем всех врагов через UnitSystem
        foreach (var pair in _unitSystem)
        {
            foreach (var unit in pair)
            {
                if (unit == null || !unit.gameObject.activeSelf) continue;

                float dist = Vector3.SqrMagnitude(unit.transform.position - transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    target = unit.transform;
                }
            }
        }
    }

    // ============================================================
    //  ВЫСТРЕЛ
    // ============================================================

    /// <summary>
    /// Создаёт стрелу и отправляет её в цель.
    /// </summary>
    private void Shoot()
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
        else
        {
            Debug.LogError("ArcherBehaviour: у стрелы нет компонента ArrowProjectile!");
        }
    }
}