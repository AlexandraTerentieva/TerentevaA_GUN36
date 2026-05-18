using System.Collections.Generic;
using UnityEngine;

public static class MoveGenerator
{
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

    static void AddPawnMoves(Unit unit, List<Cell> allCells, List<Cell> moves, int x, int z)
    {
        int dir = (unit.team == Team.White) ? 1 : -1;

        Cell forward = FindCell(allCells, x, z + dir);
        if (forward != null && forward.Unit == null)
            moves.Add(forward);

        if (!unit.HasMoved)
        {
            Cell forward2 = FindCell(allCells, x, z + dir * 2);
            Cell middle = FindCell(allCells, x, z + dir);
            if (forward2 != null && forward2.Unit == null && middle != null && middle.Unit == null)
                moves.Add(forward2);
        }

        Cell attackLeft = FindCell(allCells, x - 1, z + dir);
        if (attackLeft != null && attackLeft.Unit != null && attackLeft.Unit.team != unit.team)
            moves.Add(attackLeft);

        Cell attackRight = FindCell(allCells, x + 1, z + dir);
        if (attackRight != null && attackRight.Unit != null && attackRight.Unit.team != unit.team)
            moves.Add(attackRight);
    }

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

    static void AddRookMoves(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z)
    {
        AddLine(allCells, moves, unit, x, z, 0, 1);
        AddLine(allCells, moves, unit, x, z, 0, -1);
        AddLine(allCells, moves, unit, x, z, 1, 0);
        AddLine(allCells, moves, unit, x, z, -1, 0);
    }

    static void AddBishopMoves(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z)
    {
        AddLine(allCells, moves, unit, x, z, 1, 1);
        AddLine(allCells, moves, unit, x, z, 1, -1);
        AddLine(allCells, moves, unit, x, z, -1, 1);
        AddLine(allCells, moves, unit, x, z, -1, -1);
    }

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

    static void AddKingMoves(List<Cell> allCells, List<Cell> moves, Unit unit, int x, int z)
    {
        // Обычные ходы короля
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

        // Рокировка (только если король не двигался)
        if (unit.HasMoved) return;

        int row = (unit.team == Team.White) ? 0 : 7;

        // Короткая рокировка (H)
        Cell rookRight = FindCell(allCells, 7, row);
        Cell betweenRight1 = FindCell(allCells, 5, row);
        Cell betweenRight2 = FindCell(allCells, 6, row);
        if (rookRight != null && rookRight.Unit != null &&
            rookRight.Unit.Type == PieceType.Rook && !rookRight.Unit.HasMoved &&
            betweenRight1 != null && betweenRight1.Unit == null &&
            betweenRight2 != null && betweenRight2.Unit == null)
        {
            moves.Add(FindCell(allCells, 6, row));
        }

        // Длинная рокировка (A)
        Cell rookLeft = FindCell(allCells, 0, row);
        Cell betweenLeft1 = FindCell(allCells, 1, row);
        Cell betweenLeft2 = FindCell(allCells, 2, row);
        Cell betweenLeft3 = FindCell(allCells, 3, row);
        if (rookLeft != null && rookLeft.Unit != null &&
            rookLeft.Unit.Type == PieceType.Rook && !rookLeft.Unit.HasMoved &&
            betweenLeft1 != null && betweenLeft1.Unit == null &&
            betweenLeft2 != null && betweenLeft2.Unit == null &&
            betweenLeft3 != null && betweenLeft3.Unit == null)
        {
            moves.Add(FindCell(allCells, 2, row));
        }
    }
}