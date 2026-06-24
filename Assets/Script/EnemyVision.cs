using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Настройки обзора")]
    public float viewRadius = 5f;
    public Vector3 viewCenterOffset = new Vector3(0, 1, 0);

    private Transform enemyTransform;

    void Start()
    {
        enemyTransform = transform;
    }

    // Центр обзора в мировых координатах
    public Vector3 GetViewCenter()
    {
        return enemyTransform.position + viewCenterOffset;
    }

    // Проверка, видит ли цель
    public bool IsTargetInView(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(GetViewCenter(), targetPosition);
        return distance <= viewRadius;
    }

    // Обнаружение игрока (логика)
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && IsTargetInView(player.transform.position))
        {
            Debug.Log(gameObject.name + " видит игрока!");
        }
    }

    // Визуализация Gizmo
    void OnDrawGizmosSelected()
    {
        if (enemyTransform == null)
            enemyTransform = transform;

        Vector3 center = GetViewCenter();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, viewRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, 0.2f);
    }
}