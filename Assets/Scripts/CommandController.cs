using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер команд для управления юнитами.
/// Отвечает за патрулирование и отправку юнитов в точки.
/// </summary>
public class CommandController : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Точки патрулирования")]
    public Transform[] patrolPoints;        // Массив точек для патрулирования
    public float arrivalDistance = 0.5f;    // Дистанция, считающаяся "прибытием"

    [Header("Юниты")]
    public List<GameObject> units = new List<GameObject>(); // Список юнитов под управлением

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private int currentPointIndex = 0;      // Текущий индекс точки патрулирования

    // ============================================================
    //  МЕТОДЫ
    // ============================================================

    /// <summary>
    /// Отправить всех юнитов в указанную точку
    /// </summary>
    /// <param name="targetPosition">Целевая позиция</param>
    public void SendUnitsToPoint(Vector3 targetPosition)
    {
        foreach (var unit in units)
        {
            if (unit != null)
            {
                // Здесь будет логика движения к точке
                Debug.Log($"Юнит {unit.name} отправлен в точку {targetPosition}");
            }
        }
    }

    /// <summary>
    /// Начать патрулирование по точкам
    /// </summary>
    public void StartPatrol()
    {
        // Проверяем, есть ли точки патрулирования
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("Нет точек патрулирования!");
            return;
        }

        // Отправляем юнитов к первой точке
        currentPointIndex = 0;
        SendUnitsToPoint(patrolPoints[currentPointIndex].position);
        Debug.Log($"Патрулирование начато. Точка {currentPointIndex + 1}");
    }

    /// <summary>
    /// Перейти к следующей точке патрулирования
    /// </summary>
    public void NextPatrolPoint()
    {
        // Проверяем, есть ли точки патрулирования
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // Переход к следующей точке (с зацикливанием)
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;

        // Отправляем юнитов к новой точке
        SendUnitsToPoint(patrolPoints[currentPointIndex].position);
        Debug.Log($"Переход к точке {currentPointIndex + 1}");
    }

    // ============================================================
    //  ОТЛАДКА
    // ============================================================

    void Update()
    {
        // Отладка: нажатие P - начало патруля
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartPatrol();
        }

        // Отладка: нажатие N - следующая точка
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextPatrolPoint();
        }
    }
}