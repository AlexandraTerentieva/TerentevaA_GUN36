using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler
{
    public PieceType Type;
    public Team team;
    public bool HasMoved;
    public Cell CurrentCell;

    private Renderer unitRenderer;
    private Color originalColor;

    void Start()
    {
        Vector3 pos = transform.position;
        pos.y = 0.5f;
        transform.position = pos;

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        unitRenderer = GetComponent<Renderer>();
        if (unitRenderer != null)
            originalColor = unitRenderer.material.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[Unit] Клик по {Type} команды {team}");

        if (BattleController.Instance != null)
            BattleController.Instance.SelectUnit(this);
        else
            Debug.LogError("[Unit] BattleController.Instance = null!");
    }

    public void SetHighlight(bool active)
    {
        if (unitRenderer != null)
        {
            unitRenderer.material.color = active ? Color.yellow : originalColor;
        }
    }
}