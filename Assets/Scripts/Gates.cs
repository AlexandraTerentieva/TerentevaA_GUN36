using UnityEngine;

public class Gates : MonoBehaviour
{
    private int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>() != null)
        {
            score++;
            Debug.Log($"Ñ÷¸ò: {score}");
            Destroy(other.gameObject);
        }
    }
}