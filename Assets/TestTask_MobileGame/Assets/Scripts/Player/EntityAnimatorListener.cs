using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ������� �� ��������� (Animation Events).
/// ������������ ��� ������ ������� �� �������� (������, ���� � �.�.)
/// </summary>
public class EntityAnimatorListener : MonoBehaviour
{
    // ============================================================
    //  ������
    // ============================================================

    private CharacterEntity entity;     // ������ �� �������� ���������

    // ============================================================
    //  �����
    // ============================================================

    void Start()
    {
        entity = GetComponent<CharacterEntity>();
        if (entity == null)
        {
            Debug.LogWarning("EntityAnimatorListener: ��� CharacterEntity!");
        }
    }

    // ============================================================
    //  ������, ���������� �� ��������
    // ============================================================

    /// <summary>
    /// ���������� �� �������� � ����� �������� ������.
    /// ����� ������������ ��� �������� �������.
    /// </summary>
    public void OnDeathAnimationEnd()
    {
        Debug.Log($"{gameObject.name}: �������� ������ ���������");
        // ������ ��� ����� ����� ����� Destroy(gameObject, 2f) � CharacterEntity
    }

    /// <summary>
    /// ���������� � ������ ����� � �������� �����.
    /// ����� ����� ������� ���� �����.
    /// </summary>
    public void OnHit()
    {
        Debug.Log($"{gameObject.name}: ����!");
        // ����� ����� ������ ��������� �����
    }

    /// <summary>
    /// ���������� ��� ������ �������� �����.
    /// </summary>
    public void OnAttackStart()
    {
        Debug.Log($"{gameObject.name}: ����� ��������");
    }

    /// <summary>
    /// ���������� ��� ���������� �������� �����.
    /// </summary>
    public void OnAttackEnd()
    {
        Debug.Log($"{gameObject.name}: ����� ���������");
    }
}