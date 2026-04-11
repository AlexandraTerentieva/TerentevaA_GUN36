using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Transform _ballSpawnPoint;
    [SerializeField] private float _shootForce = 10f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootBall();
        }
    }

    private void ShootBall()
    {
        if (_ballPrefab == null)
        {
            Debug.LogError("Ball Prefab не назначен!");
            return;
        }

        GameObject ball = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);

        if (ball.GetComponent<Ball>() == null)
        {
            ball.AddComponent<Ball>();
        }

        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.AddForce(transform.forward * _shootForce, ForceMode.Impulse);
        }
    }
}