using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject _focus;
    [SerializeField] private GameObject _select;

    private List<Cell> _neighbours = new List<Cell>();
    public IReadOnlyList<Cell> Neighbours => _neighbours;

    public Unit Unit { get; set; }
    public event System.Action<Cell> OnPointerClickEvent;

    public void AddNeighbour(Cell neighbour)
    {
        if (!_neighbours.Contains(neighbour))
            _neighbours.Add(neighbour);
    }

    public bool IsNeighbour(Cell cell)
    {
        return _neighbours.Contains(cell);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_focus != null) _focus.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_focus != null) _focus.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickEvent?.Invoke(this);
    }

    public void SetSelect(Material material)
    {
        if (_select != null)
        {
            _select.SetActive(true);
            _select.GetComponent<Renderer>().material = material;
        }
    }

    public void ResetSelect()
    {
        if (_select != null) _select.SetActive(false);
    }
}