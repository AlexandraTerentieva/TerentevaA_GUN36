using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Слушает события из аниматора (Animation Events).
/// Используется для вызова методов из анимаций (смерть, удар и т.д.)
/// </summary>
public class EntityAnimatorListener : MonoBehaviour
{
    // ============================================================
    //  ССЫЛКИ
    // ============================================================

    private CharacterEntity entity;     // Ссылка на здоровье персонажа

    // ============================================================
    //  СТАРТ
    // ============================================================

    void Start()
    {
        entity = GetComponent<CharacterEntity>();
        if (entity == null)
        {
            Debug.LogWarning("EntityAnimatorListener: нет CharacterEntity!");
        }
    }

    // ============================================================
    //  МЕТОДЫ, ВЫЗЫВАЕМЫЕ ИЗ АНИМАЦИЙ
    // ============================================================

    /// <summary>
    /// Вызывается из анимации в конце анимации смерти.
    /// Можно использовать для удаления объекта.
    /// </summary>
    public void OnDeathAnimationEnd()
    {
        Debug.Log($"{gameObject.name}: анимация смерти завершена");
        // Объект уже будет удалён через Destroy(gameObject, 2f) в CharacterEntity
    }

    /// <summary>
    /// Вызывается в момент удара в анимации атаки.
    /// Здесь можно нанести урон врагу.
    /// </summary>
    public void OnHit()
    {
        Debug.Log($"{gameObject.name}: удар!");
        // Здесь будет логика нанесения урона
    }

    /// <summary>
    /// Вызывается при старте анимации атаки.
    /// </summary>
    public void OnAttackStart()
    {
        Debug.Log($"{gameObject.name}: атака началась");
    }

    /// <summary>
    /// Вызывается при завершении анимации атаки.
    /// </summary>
    public void OnAttackEnd()
    {
        Debug.Log($"{gameObject.name}: атака завершена");
    }
}