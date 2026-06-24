using UnityEngine;

public class RobotVacuum : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 2f;
    public float rotateSpeed = 100f;
    public float rayDistance = 1.5f;

    [Header("Уборка")]
    public float cleanRadius = 0.5f;
    public LayerMask trashLayer;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private float changeDirectionTimer;
    private bool isMoving = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDirection = Random.insideUnitSphere.normalized;
        moveDirection.y = 0;
    }

    void Update()
    {
        // Случайная смена направления каждые 2-5 секунд
        changeDirectionTimer -= Time.deltaTime;
        if (changeDirectionTimer <= 0)
        {
            ChangeDirection();
            changeDirectionTimer = Random.Range(2f, 5f);
        }

        // Raycast для обнаружения препятствий
        DetectObstacles();

        // Уборка мусора
        CleanTrash();

        // Движение
        Move();
    }

    void Move()
    {
        if (!isMoving) return;

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);

        // Поворот в сторону движения
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.deltaTime));
        }
    }

    void DetectObstacles()
    {
        Vector3 forward = transform.forward;
        Vector3 left = -transform.right;
        Vector3 right = transform.right;

        // Проверяем вперёд
        if (Physics.Raycast(transform.position, forward, rayDistance))
        {
            // Проверяем свободное направление
            if (!Physics.Raycast(transform.position, left, rayDistance))
                moveDirection = left;
            else if (!Physics.Raycast(transform.position, right, rayDistance))
                moveDirection = right;
            else
                moveDirection = -forward; // разворот
        }
    }

    void ChangeDirection()
    {
        Vector3 newDir = Random.insideUnitSphere.normalized;
        newDir.y = 0;
        moveDirection = newDir;
    }

    void CleanTrash()
    {
        Collider[] trash = Physics.OverlapSphere(transform.position, cleanRadius, trashLayer);
        foreach (Collider item in trash)
        {
            Destroy(item.gameObject);
            Debug.Log("Мусор собран!");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация лучей и радиуса уборки
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
        Gizmos.DrawRay(transform.position, -transform.right * rayDistance);
        Gizmos.DrawRay(transform.position, transform.right * rayDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, cleanRadius);
    }
}