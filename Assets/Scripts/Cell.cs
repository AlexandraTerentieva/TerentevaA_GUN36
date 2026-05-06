using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public Unit Unit { get; set; }
    public int X { get; set; }
    public int Z { get; set; }

    private Renderer rend;
    private Color normalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            normalColor = rend.material.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[Cell]  лик по {name}");

        if (BattleController.Instance != null)
            BattleController.Instance.MoveToCell(this);
        else
            Debug.LogError("[Cell] BattleController.Instance = null!");
    }

    public void SetHighlight(bool active)
    {
        if (rend != null)
            rend.material.color = active ? Color.green : normalColor;
    }
}