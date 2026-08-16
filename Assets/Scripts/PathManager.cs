using UnityEngine;

/// <summary>
/// Хранит точки пути для врагов.
/// </summary>
public class PathManager : MonoBehaviour
{
    [Header("Точки пути (от входа к выходу)")]
    public Transform[] waypoints;

    public Transform[] GetPath() => waypoints;
}