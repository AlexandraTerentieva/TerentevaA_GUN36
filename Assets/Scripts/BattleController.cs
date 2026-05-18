using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour, ISharedData
{
    public static BattleController Instance;

    public Team currentTurn = Team.White;
    public List<Cell> allCells = new List<Cell>();
    public List<Unit> allUnits = new List<Unit>();

    private Unit selectedUnit = null;
    private List<Cell> currentMoves = new List<Cell>();
    private PlayerController playerController;
    private PromotionUI promotionUI;
    private bool gameEnded = false;

    // Переменные для анимации перезагрузки
    private float restartTimer = 0f;
    private bool isRestarting = false;
    public Image restartFillImage;

    public GameEvent Event { get; set; }
    public Cell Target { get; set; }

    public System.Action<Team> OnTurnChanged;
    public System.Action<Team> OnGameEnded;

    void Awake()
    {
        Instance = this;
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            playerController = gameObject.AddComponent<PlayerController>();
        Event = GameEvent.None;
        promotionUI = GetComponent<PromotionUI>();
    }

    void Start()
    {
        FindAllCells();
        FindAllUnits();
        ConnectUnitsToCells();
        Debug.Log("BattleController: Игра запущена. Ходят БЕЛЫЕ (White)");
        OnTurnChanged?.Invoke(currentTurn);
    }

    void Update()
    {
        if (gameEnded) return;

        // ESC: сброс выбранной фигуры
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedUnit != null)
                selectedUnit.SetHighlight(false);
            selectedUnit = null;
            ClearHighlights();
            Debug.Log("ESC: выбор сброшен");
        }

        // SPACE: подтверждение хода
        if (Input.GetKeyDown(KeyCode.Space) && selectedUnit != null && currentMoves.Count > 0)
        {
            MakeMove(selectedUnit, currentMoves[0]);
            Debug.Log("SPACE: выполнен первый возможный ход");
        }

        // R: анимация перезагрузки с удержанием
        if (Input.GetKey(KeyCode.R))
        {
            if (!isRestarting)
            {
                isRestarting = true;
                restartTimer = 0f;
                if (restartFillImage != null)
                    restartFillImage.fillAmount = 0f;
            }

            restartTimer += Time.deltaTime;
            if (restartFillImage != null)
                restartFillImage.fillAmount = restartTimer / 1f;

            if (restartTimer >= 1f)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }
        else if (isRestarting)
        {
            isRestarting = false;
            if (restartFillImage != null)
                restartFillImage.fillAmount = 0f;
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
    }

    void FindAllUnits()
    {
        allUnits.Clear();
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
            allUnits.Add(unit);
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
            }
        }
    }

    public void SelectUnit(Unit unit)
    {
        if (gameEnded) return;
        if (playerController != null && playerController.IsBusy) return;
        if (unit.team != currentTurn) return;

        if (selectedUnit != null)
            selectedUnit.SetHighlight(false);

        selectedUnit = unit;
        selectedUnit.SetHighlight(true);
        ShowAvailableMoves(unit);
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
        moves = moves.Where(cell => IsMoveSafe(unit, cell)).ToList();

        foreach (Cell cell in moves)
        {
            currentMoves.Add(cell);
            cell.SetHighlight(true);
        }

        Debug.Log($"Найдено безопасных ходов: {currentMoves.Count}");
    }

    bool IsMoveSafe(Unit unit, Cell targetCell)
    {
        Cell oldCell = unit.CurrentCell;
        Unit targetUnit = targetCell.Unit;

        oldCell.Unit = null;
        unit.CurrentCell = targetCell;
        targetCell.Unit = unit;

        bool isSafe = !IsKingInCheck(unit.team);

        oldCell.Unit = unit;
        unit.CurrentCell = oldCell;
        targetCell.Unit = targetUnit;

        return isSafe;
    }

    bool IsKingInCheck(Team team)
    {
        Unit king = allUnits.FirstOrDefault(u => u.Type == PieceType.King && u.team == team);
        if (king == null) return false;

        foreach (Unit enemy in allUnits)
        {
            if (enemy.team == team) continue;
            List<Cell> enemyMoves = MoveGenerator.GetValidMoves(enemy, allCells);
            if (enemyMoves.Contains(king.CurrentCell))
                return true;
        }
        return false;
    }

    bool IsCheckmate(Team team)
    {
        List<Unit> friendlyUnits = allUnits.Where(u => u.team == team).ToList();

        foreach (Unit unit in friendlyUnits)
        {
            List<Cell> moves = MoveGenerator.GetValidMoves(unit, allCells);
            foreach (Cell cell in moves)
            {
                if (IsMoveSafe(unit, cell))
                    return false;
            }
        }
        return true;
    }

    void EndGame(Team winner)
    {
        gameEnded = true;
        Debug.Log($"🎉 ИГРА ОКОНЧЕНА! Победили {winner}!");
        OnGameEnded?.Invoke(winner);
    }

    void MakeMove(Unit unit, Cell targetCell)
    {
        Cell oldCell = unit.CurrentCell;
        bool isCastling = false;
        Cell rookOldCell = null;
        Cell rookNewCell = null;
        Unit rook = null;

        if (unit.Type == PieceType.King && Mathf.Abs(targetCell.X - unit.CurrentCell.X) == 2)
        {
            isCastling = true;
            int row = (unit.team == Team.White) ? 0 : 7;
            int rookX = targetCell.X > unit.CurrentCell.X ? 7 : 0;
            int rookNewX = targetCell.X > unit.CurrentCell.X ? 5 : 3;

            rookOldCell = FindCell(rookX, row);
            rookNewCell = FindCell(rookNewX, row);

            if (rookOldCell != null && rookOldCell.Unit != null && rookOldCell.Unit.Type == PieceType.Rook)
                rook = rookOldCell.Unit;
        }

        bool killedEnemyKing = false;
        if (targetCell.Unit != null && targetCell.Unit.team != unit.team)
        {
            if (targetCell.Unit.Type == PieceType.King)
                killedEnemyKing = true;
            Destroy(targetCell.Unit.gameObject);
            allUnits.Remove(targetCell.Unit);
        }

        if (killedEnemyKing)
        {
            EndGame(unit.team);
            return;
        }

        if (isCastling && rook != null)
        {
            StartCoroutine(CastlingRoutine(unit, oldCell, targetCell, rook, rookOldCell, rookNewCell));
        }
        else
        {
            playerController.ExecuteMove(unit, targetCell, () =>
            {
                oldCell.Unit = null;
                unit.CurrentCell = targetCell;
                targetCell.Unit = unit;
                unit.HasMoved = true;

                if (unit.Type == PieceType.Pawn && (targetCell.Z == 0 || targetCell.Z == 7))
                {
                    if (promotionUI != null)
                        promotionUI.Show(unit, targetCell, (selectedType) => unit.Type = selectedType);
                    else
                        unit.Type = PieceType.Queen;
                }

                if (selectedUnit != null)
                    selectedUnit.SetHighlight(false);
                selectedUnit = null;
                ClearHighlights();

                Team enemyTeam = (currentTurn == Team.White) ? Team.Black : Team.White;
                if (IsKingInCheck(enemyTeam))
                {
                    Debug.Log($"{enemyTeam} король под шахом!");
                    if (IsCheckmate(enemyTeam))
                    {
                        EndGame(currentTurn);
                        return;
                    }
                }

                SwitchTurn();
            });
        }
    }

    IEnumerator CastlingRoutine(Unit king, Cell oldKingCell, Cell newKingCell, Unit rook, Cell oldRookCell, Cell newRookCell)
    {
        yield return StartCoroutine(MoveRoutine(king, newKingCell));
        oldKingCell.Unit = null;
        king.CurrentCell = newKingCell;
        newKingCell.Unit = king;
        king.HasMoved = true;

        yield return StartCoroutine(MoveRoutine(rook, newRookCell));
        oldRookCell.Unit = null;
        rook.CurrentCell = newRookCell;
        newRookCell.Unit = rook;
        rook.HasMoved = true;

        Debug.Log("Рокировка выполнена!");

        Team enemyTeam = (currentTurn == Team.White) ? Team.Black : Team.White;
        if (IsKingInCheck(enemyTeam))
        {
            Debug.Log($"{enemyTeam} король под шахом!");
            if (IsCheckmate(enemyTeam))
            {
                EndGame(currentTurn);
                yield break;
            }
        }

        if (selectedUnit != null)
            selectedUnit.SetHighlight(false);
        selectedUnit = null;
        ClearHighlights();
        SwitchTurn();
    }

    IEnumerator MoveRoutine(Unit unit, Cell targetCell)
    {
        Vector3 start = unit.transform.position;
        Vector3 end = targetCell.transform.position + Vector3.up * 0.5f;
        float t = 0;
        float speed = 5f;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            unit.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        unit.transform.position = end;
    }

    Cell FindCell(int x, int z)
    {
        foreach (Cell cell in allCells)
            if (cell.X == x && cell.Z == z) return cell;
        return null;
    }

    public void SwitchTurn()
    {
        currentTurn = (currentTurn == Team.White) ? Team.Black : Team.White;
        Debug.Log($"Теперь ходят: {currentTurn}");
        OnTurnChanged?.Invoke(currentTurn);
    }

    void ClearHighlights()
    {
        foreach (Cell cell in allCells)
            cell.SetHighlight(false);
    }
}