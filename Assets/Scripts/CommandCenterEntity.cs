using UnityEngine;

/// <summary>
/// База, куда юниты приносят ресурсы.
/// </summary>
public class BaseEntity : MonoBehaviour
{
    [Header("Ресурсы")]
    public int woodCount = 0;
    public int stoneCount = 0;
    public int goldCount = 0;

    public void AddResource(string resourceType, int amount)
    {
        switch (resourceType)
        {
            case "Wood": woodCount += amount; Debug.Log($"🪵 Дерево: {woodCount}"); break;
            case "Stone": stoneCount += amount; Debug.Log($"🪨 Камень: {stoneCount}"); break;
            case "Gold": goldCount += amount; Debug.Log($"✨ Золото: {goldCount}"); break;
        }
    }
}