using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private float viewRadius = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private Vector3 viewCenterOffset = new Vector3(0, 1, 0);

    private Transform enemyTransform;
    private Transform playerTransform;

    void Start()
    {
        enemyTransform = transform;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    public Vector3 GetViewCenter()
    {
        return enemyTransform.position + viewCenterOffset;
    }

    public bool IsTargetInView(Vector3 targetPosition)
    {
        Vector3 center = GetViewCenter();
        Vector3 directionToTarget = targetPosition - center;

        float distance = directionToTarget.magnitude;
        if (distance > viewRadius) return false;

        float angle = Vector3.Angle(enemyTransform.forward, directionToTarget);
        return angle <= viewAngle * 0.5f;
    }

    void Update()
    {
        if (playerTransform != null && IsTargetInView(playerTransform.position))
        {
            Debug.Log(gameObject.name + " видит игрока!");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enemyTransform == null) enemyTransform = transform;

        Vector3 center = GetViewCenter();
        Vector3 forward = enemyTransform.forward;

        // Полная окружность как основа
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, viewRadius);

        // Сектор обзора (угол)
        Gizmos.color = Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * forward * viewRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle * 0.5f, 0) * forward * viewRadius;

        Gizmos.DrawLine(center, center + leftBoundary);
        Gizmos.DrawLine(center, center + rightBoundary);

        // Дуга сектора
        float angleStep = 5f;
        for (float angle = -viewAngle * 0.5f; angle <= viewAngle * 0.5f; angle += angleStep)
        {
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward * viewRadius;
            Gizmos.DrawLine(center + dir, center + Quaternion.Euler(0, angle + angleStep, 0) * forward * viewRadius);
        }
    }
}