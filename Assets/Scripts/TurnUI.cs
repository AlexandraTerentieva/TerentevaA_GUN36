using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    public TMP_Text turnText;

    void Start()
    {
        if (BattleController.Instance != null)
        {
            UpdateTurn(BattleController.Instance.currentTurn);
            BattleController.Instance.OnTurnChanged += UpdateTurn;
        }
        else
        {
            Debug.LogError("TurnUI: BattleController.Instance не найден!");
        }
    }

    void UpdateTurn(Team team)
    {
        if (turnText != null)
        {
            turnText.text = "Ход: " + (team == Team.White ? "Белые" : "Чёрные");
            Debug.Log($"TurnUI: текст обновлён на {turnText.text}");
        }
        else
        {
            Debug.LogError("TurnUI: turnText не назначен!");
        }
    }

    void OnDestroy()
    {
        if (BattleController.Instance != null)
            BattleController.Instance.OnTurnChanged -= UpdateTurn;
    }
}