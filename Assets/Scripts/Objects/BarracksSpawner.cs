using UnityEngine;
using Netologia.TowerDefence;

/// <summary>
/// Казарма. Призывает лучников.
/// Количество лучников = текущий уровень казармы (максимум 4).
/// </summary>
public class BarracksSpawner : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ (заполняются в инспекторе)
    // ============================================================

    [Header("Настройки спавна")]
    [SerializeField] private GameObject archerPrefab;   // Префаб лучника
    [SerializeField] private float spawnDelay = 2f;     // Задержка между появлением

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
        tower = GetComponent<Tower>();
        if (tower == null)
        {
            Debug.LogError("BarracksSpawner: нет компонента Tower!");
            return;
        }

        spawnTimer = spawnDelay;
    }

    // ============================================================
    //  ЛОГИКА КАЖДЫЙ КАДР
    // ============================================================

    void Update()
    {
        if (archerPrefab == null) return;

        int maxArchers = GetMaxArchers();
        if (currentArchers >= maxArchers) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnArcher();
            spawnTimer = spawnDelay;
        }
    }

    // ============================================================
    //  МЕТОДЫ
    // ============================================================

    /// <summary>
    /// Максимальное количество лучников = текущий уровень казармы (1-4)
    /// </summary>
    private int GetMaxArchers()
    {
        int level = tower.Level + 1;
        return Mathf.Min(level, 4);
    }

    /// <summary>
    /// Создаёт лучника рядом с казармой
    /// </summary>
    private void SpawnArcher()
    {
        Vector3 spawnPos = transform.position + new Vector3(1.5f, 0, 0);
        GameObject archer = Instantiate(archerPrefab, spawnPos, Quaternion.identity);
        currentArchers++;
    }

    /// <summary>
    /// Вызывается при смерти лучника
    /// </summary>
    public void OnArcherDied()
    {
        currentArchers--;
        if (currentArchers < 0) currentArchers = 0;
    }

    /// <summary>
    /// Вызывается при улучшении казармы (из Tower.LevelUp)
    /// </summary>
    public void OnUpgrade()
    {
        // Можно добавить логику, если нужно
    }
}