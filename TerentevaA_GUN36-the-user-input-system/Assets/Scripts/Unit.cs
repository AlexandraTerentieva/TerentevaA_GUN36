using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public Cell Cell { get; set; }
    public event System.Action OnMoveEndCallback;

    private void Start()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    public void Move(Cell targetCell)
    {
        Debug.Log($"ƒвигаю юнита с {Cell.name} на {targetCell.name}");
        StartCoroutine(MoveRoutine(targetCell));
    }

    private IEnumerator MoveRoutine(Cell target)
    {
        Vector3 start = transform.position;
        Vector3 end = target.transform.position;
        float t = 0;
        float speed = 5f;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
        Cell = target;
        OnMoveEndCallback?.Invoke();
    }
}