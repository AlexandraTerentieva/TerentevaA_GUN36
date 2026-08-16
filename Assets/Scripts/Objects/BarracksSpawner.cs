using UnityEngine;
using Netologia.TowerDefence;

/// <summary>
/// Казарма. Призывает лучников.
/// Количество лучников = текущий уровень казармы (максимум 4).
/// На 1 уровне — 1 лучник, на 2 — 2, на 3 — 3, на 4 — 4.
/// </summary>
public class BarracksSpawner : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ (заполняются в инспекторе)
    // ============================================================

    [Header("Настройки спавна")]
    public GameObject archerPrefab;   // Префаб лучника (перетащить в инспекторе)
    public float spawnDelay = 2f;     // Задержка между появлением лучников

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private Tower tower;              // Ссылка на компонент Tower (чтобы узнать уровень)
    private int currentArchers = 0;   // Сколько лучников уже призвано
    private float spawnTimer = 0f;    // Таймер до следующего спавна

    // ============================================================
    //  ЛОГИКА ПРИ ЗАПУСКЕ
    // ============================================================

    void Start()
    {
        // Получаем компонент Tower (нужен для уровня)
        tower = GetComponent<Tower>();

        // Если Tower нет — ошибка
        if (tower == null)
        {
            Debug.LogError("BarracksSpawner: нет компонента Tower!");
            return;
        }

        // Запускаем таймер
        spawnTimer = spawnDelay;
    }

    // ============================================================
    //  ЛОГИКА КАЖДЫЙ КАДР
    // ============================================================

    void Update()
    {
        // Если нет префаба лучника — ничего не делаем
        if (archerPrefab == null) return;

        // Сколько максимум можно призвать на текущем уровне
        int maxArchers = GetMaxArchers();

        // Если уже достигли максимума — ждём
        if (currentArchers >= maxArchers) return;

        // Уменьшаем таймер
        spawnTimer -= Time.deltaTime;

        // Если таймер закончился — призываем лучника
        if (spawnTimer <= 0f)
        {
            SpawnArcher();
            spawnTimer = spawnDelay; // Сбрасываем таймер
        }
    }

    // ============================================================
    //  МЕТОДЫ
    // ============================================================

    /// <summary>
    /// Максимальное количество лучников = текущий уровень казармы (1-4)
    /// </summary>
    int GetMaxArchers()
    {
        int level = tower.Level + 1;          // Level: 0,1,2,3 → 1,2,3,4
        int max = Mathf.Min(level, 4);        // Не больше 4

        // Отладка (чтобы видеть в консоли)
        Debug.Log($"Уровень казармы: {level}, максимум лучников: {max}, сейчас: {currentArchers}");
        return max;
    }

    /// <summary>
    /// Создаёт лучника рядом с казармой
    /// </summary>
    void SpawnArcher()
    {
        // Позиция спавна — рядом с казармой
        Vector3 spawnPos = transform.position + new Vector3(1.5f, 0, 0);

        // Создаём лучника
        GameObject archer = Instantiate(archerPrefab, spawnPos, Quaternion.identity);

        // Увеличиваем счётчик
        currentArchers++;

        // Лог в консоль
        Debug.Log($"Лучник призван! Всего: {currentArchers}");
    }

    /// <summary>
    /// Вызывается при улучшении казармы (из Tower.LevelUp)
    /// </summary>
    public void OnUpgrade()
    {
        Debug.Log($"Казарма улучшена! Уровень: {tower.Level + 1}");
        // currentArchers не сбрасываем — новые лучники будут спавниться автоматически
    }
}