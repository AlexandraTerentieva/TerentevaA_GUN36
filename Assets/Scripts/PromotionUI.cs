using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    public GameObject panel;
    public Button queenButton;
    public Button rookButton;
    public Button bishopButton;
    public Button knightButton;

    private Unit currentPawn;
    private Cell currentCell;
    private System.Action<PieceType> onComplete;

    void Start()
    {
        queenButton.onClick.AddListener(() => SelectPiece(PieceType.Queen));
        rookButton.onClick.AddListener(() => SelectPiece(PieceType.Rook));
        bishopButton.onClick.AddListener(() => SelectPiece(PieceType.Bishop));
        knightButton.onClick.AddListener(() => SelectPiece(PieceType.Knight));
        panel.SetActive(false);
    }

    public void Show(Unit pawn, Cell cell, System.Action<PieceType> callback)
    {
        currentPawn = pawn;
        currentCell = cell;
        onComplete = callback;
        panel.SetActive(true);
    }

    void SelectPiece(PieceType type)
    {
        panel.SetActive(false);
        onComplete?.Invoke(type);
    }
}