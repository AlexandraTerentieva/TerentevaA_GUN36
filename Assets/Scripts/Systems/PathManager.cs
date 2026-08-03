using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Автоматический сбор точек пути с PathCell
/// </summary>
public class PathManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Transform startPoint;   // SpecialCell вход
    [SerializeField] private Transform endPoint;     // SpecialCell выход

    private Transform[] waypoints;

    private void Start()
    {
        BuildPath();
    }

    private void BuildPath()
    {
        // Находим все Waypoint на сцене
        List<Transform> allWaypoints = new List<Transform>();

        // Ищем все объекты с именем "Waypoint"
        foreach (Transform child in FindObjectsOfType<Transform>())
        {
            if (child.name == "Waypoint")
            {
                allWaypoints.Add(child);
            }
        }

        if (allWaypoints.Count == 0)
        {
            Debug.LogError("Waypoint не найдены! Добавьте их в префаб PathCell");
            return;
        }

        // Сортируем по расстоянию от старта
        Vector3 startPos = startPoint != null ? startPoint.position : Vector3.zero;

        waypoints = allWaypoints
            .OrderBy(w => Vector3.Distance(w.position, startPos))
            .ToArray();

        Debug.Log($"Собрано {waypoints.Length} точек пути автоматически");
    }

    public Transform[] GetPath()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            BuildPath();
        }
        return waypoints;
    }
}