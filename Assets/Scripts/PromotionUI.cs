using UnityEngine;
using UnityEngine.UI;
using System;

public class PromotionUI : MonoBehaviour
{
    [SerializeField] private GameObject promotionPanel;
    [SerializeField] private Button queenButton;
    [SerializeField] private Button rookButton;
    [SerializeField] private Button bishopButton;
    [SerializeField] private Button knightButton;

    private Unit currentPawn;
    private Cell targetCell;
    private Action<PieceType> onPieceSelected; // ← исправлено

    private void Start()
    {
        promotionPanel.SetActive(false);

        queenButton.onClick.AddListener(() => SelectPiece(PieceType.Queen));
        rookButton.onClick.AddListener(() => SelectPiece(PieceType.Rook));
        bishopButton.onClick.AddListener(() => SelectPiece(PieceType.Bishop));
        knightButton.onClick.AddListener(() => SelectPiece(PieceType.Knight));
    }

    public void Show(Unit pawn, Cell cell, Action<PieceType> callback)
    {
        currentPawn = pawn;
        targetCell = cell;
        onPieceSelected = callback;
        promotionPanel.SetActive(true);
    }

    private void SelectPiece(PieceType type) // ← исправлено
    {
        promotionPanel.SetActive(false);
        onPieceSelected?.Invoke(type);
    }
}