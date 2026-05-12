using UnityEngine;
using System.Collections.Generic;

public class BattleController : MonoBehaviour, ISharedData
{
    public static BattleController Instance;

    public Team currentTurn = Team.White;
    public List<Cell> allCells = new List<Cell>();
    public List<Unit> allUnits = new List<Unit>();

    private Unit selectedUnit = null;
    private List<Cell> currentMoves = new List<Cell>();
    private PlayerController playerController;

    // Реализация ISharedData
    public GameEvent Event { get; set; }
    public Cell Target { get; set; }

    void Awake()
    {
        Instance = this;
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            playerController = gameObject.AddComponent<PlayerController>();
        Event = GameEvent.None;
    }

    void Start()
    {
        FindAllCells();
        FindAllUnits();
        ConnectUnitsToCells();
        Debug.Log("BattleController: Игра запущена. Ходят БЕЛЫЕ (White)");
    }

    void Update()
    {
        // Обрабатываем событие NewTurn от чита
        if (Event == GameEvent.NewTurn)
        {
            SwitchTurn();
            Event = GameEvent.None;
        }

        // Обрабатываем событие PerformAttack от чита (опционально)
        if (Event == GameEvent.PerformAttack && Target != null && Target.Unit != null)
        {
            if (Target.Unit.team != currentTurn)
            {
                DestroyImmediate(Target.Unit.gameObject);
                Target.Unit = null;
                Debug.Log($"[ISharedData] Убита фигура через Event");
            }
            Event = GameEvent.None;
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
        Debug.Log($"Найдено клеток: {allCells.Count}");
    }

    void FindAllUnits()
    {
        allUnits.Clear();
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            allUnits.Add(unit);
        }
        Debug.Log($"Найдено фигур: {allUnits.Count}");
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

                Debug.Log($"Связь: {unit.Type} {unit.team} на {closestCell.name}");
            }
        }
    }

    public void SelectUnit(Unit unit)
    {
        if (playerController != null && playerController.IsBusy) return;
        if (unit.team != currentTurn) return;

        selectedUnit = unit;
        ShowAvailableMoves(unit);
        Debug.Log($"Выбрана фигура {unit.Type}");
    }

    public void MoveToCell(Cell cell)
    {
        if (selectedUnit == null) return;
        if (!currentMoves.Contains(cell)) return;

        MakeMove(selectedUnit, cell);
    }

    void ShowAvailableMoves(Unit unit)
    {
        ClearHighlights();
        currentMoves.Clear();

        if (unit.CurrentCell == null) return;

        List<Cell> moves = MoveGenerator.GetValidMoves(unit, allCells);

        foreach (Cell cell in moves)
        {
            currentMoves.Add(cell);
            cell.SetHighlight(true);
        }

        Debug.Log($"Найдено ходов: {currentMoves.Count}");
    }

    void MakeMove(Unit unit, Cell targetCell)
    {
        Cell oldCell = unit.CurrentCell;

        // Обновляем ISharedData
        Event = GameEvent.PerformMove;
        Target = targetCell;

        if (targetCell.Unit != null && targetCell.Unit.team != unit.team)
        {
            Event = GameEvent.PerformAttack;
            Target = targetCell;
            Debug.Log($"[ISharedData] Убита фигура {targetCell.Unit.Type}");
            Destroy(targetCell.Unit.gameObject);
        }

        playerController.ExecuteMove(unit, targetCell, () =>
        {
            oldCell.Unit = null;
            unit.HasMoved = true;

            if (unit.Type == PieceType.Pawn && (targetCell.Z == 0 || targetCell.Z == 7))
                unit.Type = PieceType.Queen;

            SwitchTurn();
        });

        selectedUnit = null;
        ClearHighlights();
    }

    public void SwitchTurn()
    {
        currentTurn = (currentTurn == Team.White) ? Team.Black : Team.White;
        Event = GameEvent.NewTurn;
        Debug.Log($"[ISharedData] NewTurn! Теперь ходят: {currentTurn}");
    }

    void ClearHighlights()
    {
        foreach (Cell cell in allCells)
            cell.SetHighlight(false);
    }
}