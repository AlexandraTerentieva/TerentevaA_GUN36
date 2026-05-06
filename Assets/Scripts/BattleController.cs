using UnityEngine;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    public Team currentTurn = Team.White;
    public List<Cell> allCells = new List<Cell>();
    public List<Unit> allUnits = new List<Unit>();

    private Unit selectedUnit = null;
    private List<Cell> currentMoves = new List<Cell>();
    private PlayerController playerController;

    void Awake()
    {
        Instance = this;
        Debug.Log("[BattleController] Awake - Instance создан");
    }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            playerController = gameObject.AddComponent<PlayerController>();

        FindAllCells();
        FindAllUnits();
        ConnectUnitsToCells();
        Debug.Log("[BattleController] Игра запущена. Ходят БЕЛЫЕ (White)");
    }

    void Update()
    {
        // Cancel (ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedUnit != null)
                selectedUnit.SetHighlight(false);
            selectedUnit = null;
            ClearHighlights();
            Debug.Log("[Cancel] Выбор сброшен");
        }
    }

    void FindAllCells()
    {
        allCells.Clear();
        Cell[] cells = FindObjectsOfType<Cell>();
        foreach (Cell cell in cells)
        {
            allCells.Add(cell);
            string[] parts = cell.name.Split('_');
            if (parts.Length == 3)
            {
                cell.X = int.Parse(parts[1]);
                cell.Z = int.Parse(parts[2]);
            }
        }
        Debug.Log($"[BattleController] Найдено клеток: {allCells.Count}");
    }

    void FindAllUnits()
    {
        allUnits.Clear();
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            allUnits.Add(unit);
        }
        Debug.Log($"[BattleController] Найдено фигур: {allUnits.Count}");
    }

    void ConnectUnitsToCells()
    {
        foreach (Unit unit in allUnits)
        {
            Cell closestCell = null;
            float minDist = 1f;

            foreach (Cell cell in allCells)
            {
                float dist = Vector3.Distance(unit.transform.position, cell.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestCell = cell;
                }
            }

            if (closestCell != null)
            {
                unit.CurrentCell = closestCell;
                closestCell.Unit = unit;
                Vector3 newPos = closestCell.transform.position;
                newPos.y = 0.5f;
                unit.transform.position = newPos;

                if (unit.team != Team.White && unit.team != Team.Black)
                    unit.team = closestCell.Z < 4 ? Team.White : Team.Black;

                Debug.Log($"[Связь] {unit.Type} {unit.team} на {closestCell.name}");
            }
        }
    }

    public void SelectUnit(Unit unit)
    {
        Debug.Log($"[SelectUnit] Вызван для {unit.Type} {unit.team}. Текущий ход: {currentTurn}");

        if (playerController != null && playerController.IsBusy)
        {
            Debug.Log("  Анимация, подождите");
            return;
        }

        if (unit.team != currentTurn)
        {
            Debug.Log($"  НЕЛЬЗЯ: ходят {currentTurn}");
            return;
        }

        if (selectedUnit != null)
            selectedUnit.SetHighlight(false);

        selectedUnit = unit;
        selectedUnit.SetHighlight(true);

        ShowAvailableMoves(unit);
        Debug.Log($"  ВЫБРАНА фигура {unit.Type}");
    }

    public void MoveToCell(Cell cell)
    {
        Debug.Log($"[MoveToCell] Вызван для {cell.name}");

        if (selectedUnit == null)
        {
            Debug.Log("  Нет выбранной фигуры");
            return;
        }

        if (!currentMoves.Contains(cell))
        {
            Debug.Log($"  Клетка {cell.name} не в списке доступных");
            return;
        }

        Debug.Log($"  Ход разрешён!");
        MakeMove(selectedUnit, cell);
    }

    void ShowAvailableMoves(Unit unit)
    {
        ClearHighlights();
        currentMoves.Clear();

        if (unit.CurrentCell == null)
        {
            Debug.LogError($"У {unit.Type} нет CurrentCell!");
            return;
        }

        List<Cell> moves = MoveGenerator.GetValidMoves(unit, allCells);

        Debug.Log($"[ХОДЫ] Для {unit.Type} найдено: {moves.Count}");

        foreach (Cell cell in moves)
        {
            currentMoves.Add(cell);
            cell.SetHighlight(true);
            Debug.Log($"  + {cell.name} ({cell.X},{cell.Z})");
        }
    }

    void MakeMove(Unit unit, Cell targetCell)
    {
        Debug.Log($"[MakeMove] {unit.Type} -> {targetCell.name}");

        Cell oldCell = unit.CurrentCell;

        if (targetCell.Unit != null && targetCell.Unit.team != unit.team)
        {
            Debug.Log($"  Убита {targetCell.Unit.Type}");
            Destroy(targetCell.Unit.gameObject);
        }

        // Убираем подсветку с выбранной фигуры перед ходом
        if (selectedUnit != null)
            selectedUnit.SetHighlight(false);

        playerController.ExecuteMove(unit, targetCell, () =>
        {
            oldCell.Unit = null;
            unit.HasMoved = true;

            if (unit.Type == PieceType.Pawn && (targetCell.Z == 0 || targetCell.Z == 7))
            {
                unit.Type = PieceType.Queen;
                Debug.Log("  Пешка -> Ферзь!");
            }

            SwitchTurn();
        });

        selectedUnit = null;
        ClearHighlights();
    }

    void ClearHighlights()
    {
        foreach (Cell cell in allCells)
            cell.SetHighlight(false);
    }

    void SwitchTurn()
    {
        currentTurn = (currentTurn == Team.White) ? Team.Black : Team.White;
        Debug.Log($"========== Ход переключён: {currentTurn} ==========");
    }
}