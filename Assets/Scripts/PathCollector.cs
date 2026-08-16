using UnityEngine;
using System.Collections.Generic;
using Netologia.Systems;

/// <summary>
/// Автоматически собирает все Waypoint на сцене и передаёт их в UnitSystem.
/// </summary>
public class PathCollector : MonoBehaviour
{
    [Header("Ссылки")]
    public UnitSystem unitSystem;
    public Transform startPoint;

    private void Start()
    {
        CollectAndSetPath();
    }

    private void CollectAndSetPath()
    {
        // Находим все объекты с именем "Waypoint"
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<Transform> waypoints = new List<Transform>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith("Waypoint"))
            {
                waypoints.Add(obj.transform);
            }
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("Waypoint не найдены! Создай объекты с именем Waypoint_0, Waypoint_1...");
            return;
        }

        // Сортируем по имени (Waypoint_0, Waypoint_1...)
        waypoints.Sort((a, b) => a.name.CompareTo(b.name));

        // Если есть стартовая точка — перестраиваем путь от неё
        if (startPoint != null)
        {
            waypoints.Sort((a, b) =>
                Vector3.Distance(a.position, startPoint.position)
                .CompareTo(Vector3.Distance(b.position, startPoint.position)));
        }

        // Преобразуем Transform[] в Vector3[]
        Vector3[] pathPositions = new Vector3[waypoints.Count];
        for (int i = 0; i < waypoints.Count; i++)
        {
            pathPositions[i] = waypoints[i].position;
        }

        // Передаём путь в UnitSystem
        if (unitSystem != null)
        {
            unitSystem.SetPath(pathPositions);
            Debug.Log($"Путь собран и передан: {waypoints.Count} точек");
        }
        else
        {
            Debug.LogError("UnitSystem не назначен в PathCollector!");
        }
    }
}