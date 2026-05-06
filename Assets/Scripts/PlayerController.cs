using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public bool IsBusy { get; private set; }

    public void ExecuteMove(Unit unit, Cell targetCell, System.Action onComplete)
    {
        StartCoroutine(MoveRoutine(unit, targetCell, onComplete));
    }

    private IEnumerator MoveRoutine(Unit unit, Cell targetCell, System.Action onComplete)
    {
        IsBusy = true;

        Vector3 start = unit.transform.position;
        Vector3 end = targetCell.transform.position;
        end.y = 0.5f;

        float t = 0;
        float speed = 5f;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            unit.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        unit.transform.position = end;
        unit.CurrentCell = targetCell;
        targetCell.Unit = unit;

        IsBusy = false;
        onComplete?.Invoke();
    }
}