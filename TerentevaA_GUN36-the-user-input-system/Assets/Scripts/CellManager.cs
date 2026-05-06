using UnityEngine;
using System.Collections.Generic;

public class CellManager : MonoBehaviour
{
    [SerializeField] private CellPaletteSettings _palette;
    private List<Cell> _cells = new List<Cell>();
    private Cell _currentSelectedCell;

    private void Start()
    {
        FindAllCells();
        FindNeighboursForAllCells();
        FindAndLinkUnits();
    }

    private void FindAllCells()
    {
        _cells.Clear();
        _cells.AddRange(FindObjectsOfType<Cell>());
        foreach (var cell in _cells)
        {
            cell.OnPointerClickEvent += OnCellClick;
        }
        Debug.Log($"Найдено клеток: {_cells.Count}");
    }

    private void FindNeighboursForAllCells()
    {
        foreach (var cell in _cells)
        {
            string[] parts = cell.name.Split('_');
            if (parts.Length < 3) continue;
            int x = int.Parse(parts[1]);
            int z = int.Parse(parts[2]);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0) continue;
                    TryAddNeighbour(cell, x + dx, z + dz);
                }
            }
        }
        Debug.Log("Соседи найдены для всех клеток");
    }

    private void TryAddNeighbour(Cell cell, int x, int z)
    {
        if (x < 0 || x >= 8 || z < 0 || z >= 8) return;
        string neighbourName = $"Cell_{x}_{z}";
        Cell neighbour = _cells.Find(c => c.name == neighbourName);
        if (neighbour != null)
            cell.AddNeighbour(neighbour);
    }

    private void FindAndLinkUnits()
    {
        Unit[] units = FindObjectsOfType<Unit>();
        Debug.Log($"Найдено юнитов: {units.Length}");
        foreach (var unit in units)
        {
            foreach (var cell in _cells)
            {
                if (Vector3.Distance(unit.transform.position, cell.transform.position) < 0.5f)
                {
                    unit.Cell = cell;
                    cell.Unit = unit;
                    Debug.Log($"Юнит {unit.name} привязан к клетке {cell.name}");
                    break;
                }
            }
        }
    }

    private void OnCellClick(Cell cell)
    {
        // Перемещение юнита
        if (_currentSelectedCell != null && _currentSelectedCell.Unit != null)
        {
            if (_currentSelectedCell.IsNeighbour(cell))
            {
                _currentSelectedCell.Unit.Move(cell);
                cell.Unit = _currentSelectedCell.Unit;
                _currentSelectedCell.Unit = null;
                _currentSelectedCell.ResetSelect();
                foreach (var n in _currentSelectedCell.Neighbours)
                    n.ResetSelect();
                _currentSelectedCell = null;
                return;
            }
        }

        // Сброс выделения с предыдущей клетки
        if (_currentSelectedCell != null)
        {
            _currentSelectedCell.ResetSelect();
            foreach (var n in _currentSelectedCell.Neighbours)
                n.ResetSelect();
        }

        // Выделение новой клетки
        _currentSelectedCell = cell;
        _currentSelectedCell.SetSelect(_palette.selectedMaterial);

        // Подсветка соседей, если на клетке есть юнит
        if (_currentSelectedCell.Unit != null)
        {
            foreach (var n in _currentSelectedCell.Neighbours)
            {
                n.SetSelect(_palette.canMoveMaterial);
            }
        }
    }
}