using System.Collections.Generic;
using UnityEngine;

public static class MoveGenerator
{
    // Получить все доступные ходы для фигуры
    public static List<Cell> GetValidMoves(Unit unit, List<Cell> allCells)
    {
        List<Cell> moves = new List<Cell>();

        if (unit == null || unit.CurrentCell == null) return moves;

        int x = unit.CurrentCell.X;
        int z = unit.CurrentCell.Z;

        switch (unit.Type)
        {
            case PieceType.Pawn:
                AddPawnMoves(unit, allCells, moves, x, z);
                break;
            case PieceType.Knight:
                AddKnightMoves(allCells, moves, unit, x, z);
                break;
            case PieceType.Rook:
                AddRookMoves(allCells, moves, unit, x, z);
                break;
            case PieceType.Bishop:
                AddBishopMoves(allCells, moves, unit, x, z);
                break;
            case PieceType.Queen:
                AddRookMoves(allCells, moves, unit, x, z);
                AddBishopMoves(allCells, moves, unit, x, z);
                break;
            case PieceType.King:
                AddKingMoves(allCells, moves, unit, x, z);
                break;
        }

        return moves;
    }

    static Cell FindCell(List<Cell> cells, int x, int z)
    {
        foreach (Cell cell in cells)
            if (cell.X == x && cell.Z == z) return cell;
        return null;
    }

    // ==================== ПЕШКА ====================
    static void AddPawnMoves(Unit unit, List<Cell> allCells, List<Cell> moves, int x, int z)
    {
        int direction = (unit.team == Team.White) ? 1 : -1;

        // Ход вперёд на 1 клетку
        Cell forward = FindCell(allCells, x, z + direction);
        if (forward != null && forward.Unit == null)
            moves.Add(forward);

        // Первый ход на 2 клетки
        if (!unit.HasMoved)
        {
            Cell forward2 = FindCell(allCells, x, z + direction * 2);
            Cell middle = FindCell(allCells, x, z + direction);
            if (forward2 != null && forward2.Unit == null && middle != null && middle.Unit == null)
                moves.Add(forward2);
        }

        // Атака по диагонали влево
        Cell attackLeft = FindCell(allCells, x - 1, z + direction);
        if (attackLeft != null && attackLeft.Unit != null && attackLeft.Unit.team != unit.team)
            moves.Add(attackLeft);

        // Атака по диагонали вправо
        Cell attackRight = FindCell(allCells, x + 1, z + direction);
        if (attackRight != null && attackRight.Unit != null && attackRight.Unit.team != unit.team)
            moves.Add(attackRight);
    }

    // ==================== КОНЬ ====================
    static void AddKnightMoves(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z)
    {
        int[] dx = { 2, 2, 1, 1, -1, -1, -2, -2 };
        int[] dz = { 1, -1, 2, -2, 2, -2, 1, -1 };

        for (int i = 0; i < 8; i++)
        {
            Cell cell = FindCell(allCells, x + dx[i], z + dz[i]);
            if (cell != null && (cell.Unit == null || cell.Unit.team != unit.team))
                moves.Add(cell);
        }
    }

    // ==================== ЛАДЬЯ ====================
    static void AddRookMoves(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z)
    {
        AddLine(allCells, moves, unit, x, z, 0, 1);   // вверх
        AddLine(allCells, moves, unit, x, z, 0, -1);  // вниз
        AddLine(allCells, moves, unit, x, z, 1, 0);   // вправо
        AddLine(allCells, moves, unit, x, z, -1, 0);  // влево
    }

    // ==================== СЛОН ====================
    static void AddBishopMoves(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z)
    {
        AddLine(allCells, moves, unit, x, z, 1, 1);    // вверх-вправо
        AddLine(allCells, moves, unit, x, z, 1, -1);   // вниз-вправо
        AddLine(allCells, moves, unit, x, z, -1, 1);   // вверх-влево
        AddLine(allCells, moves, unit, x, z, -1, -1);  // вниз-влево
    }

    // ==================== КОРОЛЬ ====================
    static void AddKingMoves(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;
                Cell cell = FindCell(allCells, x + dx, z + dz);
                if (cell != null && (cell.Unit == null || cell.Unit.team != unit.team))
                    moves.Add(cell);
            }
        }
    }

    // ==================== ЛИНИЯ (для ладьи, слона, ферзя) ====================
    static void AddLine(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z, int dx, int dz)
    {
        int nx = x + dx;
        int nz = z + dz;

        while (true)
        {
            Cell cell = FindCell(allCells, nx, nz);
            if (cell == null) break;

            if (cell.Unit == null)
            {
                moves.Add(cell);
            }
            else
            {
                if (cell.Unit.team != unit.team)
                    moves.Add(cell);
                break;
            }

            nx += dx;
            nz += dz;
        }
    }
}