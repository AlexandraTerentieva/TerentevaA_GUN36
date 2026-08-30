using UnityEngine;

/// <summary>
/// База, куда юниты приносят ресурсы.
/// </summary>
public class BaseEntity : MonoBehaviour
{
    // ============================================================
    //  РЕСУРСЫ НА БАЗЕ
    // ============================================================

    public int woodCount = 0;     // Количество древесины
    public int stoneCount = 0;    // Количество камня
    public int goldCount = 0;     // Количество золота

    // ============================================================
    //  МЕТОД: ДОБАВИТЬ РЕСУРС
    // ============================================================

    /// <summary>
    /// Добавляет ресурс на базу.
    /// </summary>
    /// <param name="resourceType">Тип ресурса: "Wood", "Stone", "Gold"</param>
    /// <param name="amount">Количество</param>
    public void AddResource(string resourceType, int amount)
    {
        switch (resourceType)
        {
            case "Wood":
                woodCount += amount;
                Debug.Log($"🪵 Древесина: {woodCount}");
                break;

            case "Stone":
                stoneCount += amount;
                Debug.Log($"🪨 Камень: {stoneCount}");
                break;

            case "Gold":
                goldCount += amount;
                Debug.Log($"✨ Золото: {goldCount}");
                break;

            default:
                Debug.LogWarning($"⚠️ Неизвестный тип ресурса: {resourceType}");
                break;
        }
    }

    // ============================================================
    //  МЕТОД: ПОКАЗАТЬ РЕСУРСЫ (для UI)
    // ============================================================

    public string GetResourcesInfo()
    {
        return $"Древесина: {woodCount}, Камень: {stoneCount}, Золото: {goldCount}";
    }
}