using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Холм с ресурсами. Юнит может собрать ресурс.
/// Холм остаётся на месте, даже когда ресурс закончился.
/// </summary>
public class ResourceHill : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Тип ресурса")]
    public string resourceType = "Wood";   // Wood, Stone, Gold

    [Header("Количество")]
    public int resourceAmount = 10;        // Сколько ресурса в холме

    // ============================================================
    //  МЕТОДЫ
    // ============================================================

    /// <summary>
    /// Собрать ресурс с холма
    /// </summary>
    public int CollectResource()
    {
        if (resourceAmount <= 0) return 0;

        int collected = 1; // Собираем по 1 единице за раз
        resourceAmount -= collected;
        Debug.Log($"✅ Собрано {collected} {resourceType}. Осталось: {resourceAmount}");

        // ❌ ХОЛМ НЕ УДАЛЯЕТСЯ!
        // Он просто остаётся с resourceAmount = 0

        return collected;
    }

    /// <summary>
    /// Проверка, есть ли ресурс
    /// </summary>
    public bool HasResource()
    {
        return resourceAmount > 0;
    }
}