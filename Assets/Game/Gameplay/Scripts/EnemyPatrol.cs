using UnityEngine;
using Game.GameEngine.Ecs;
using SampleProject;

public class EnemyPatrol : MonoBehaviour
{
    private Entity entity;                // Entity врага
    private Transform[] patrolPoints;     // Массив точек патрулирования
    private int currentPointIndex = 0;    // Индекс текущей точки
    private bool isPatrolling = true;     // Патрулируем?
    private float reachDistance = 1.5f;   // Расстояние до точки
    private bool isWaiting = false;       // Стоим на точке?
    private float waitTime = 4f;          // Сколько секунд стоим
    private float waitTimer = 0f;         // Таймер ожидания

    private bool isChasing = false;       // Преследуем игрока?
    private Entity playerEntity;          // Entity игрока

    private void Start()
    {
        entity = GetComponent<Entity>();

        // Ищем все точки патрулирования по имени
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        System.Collections.Generic.List<Transform> pointsList = new System.Collections.Generic.List<Transform>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("PatrolPoint"))
            {
                pointsList.Add(obj.transform);
            }
        }

        patrolPoints = pointsList.ToArray();

        if (patrolPoints.Length > 0)
        {
            MoveToPoint(currentPointIndex);
        }
    }

    // --- МЕТОД UPDATE: ПЕРЕКЛЮЧАЕТ ТОЧКИ И ПРОВЕРЯЕТ ЦЕЛЬ ---
    private void Update()
    {
        // Если преследуем игрока — проверяем, жив ли он (по GameObject)
        if (isChasing)
        {
            // Если игрок исчез или неактивен — возвращаемся к патрулированию
            if (playerEntity == null || playerEntity.gameObject == null || !playerEntity.gameObject.activeInHierarchy)
            {
                isChasing = false;
                isPatrolling = true;
                StopAttack();
                MoveToPoint(currentPointIndex);
                return;
            }
        }

        // Если патрулирование выключено или нет точек — выходим
        if (!isPatrolling || patrolPoints.Length == 0) return;

        // Если преследуем игрока — не патрулируем
        if (isChasing) return;

        // Если ждём на точке — уменьшаем таймер
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                MoveToPoint(currentPointIndex);
            }
            return;
        }

        // Проверяем расстояние до текущей точки
        Transform targetPoint = patrolPoints[currentPointIndex];
        float distance = Vector3.Distance(transform.position, targetPoint.position);

        // Если дошли до точки — начинаем ждать
        if (distance < reachDistance)
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    // --- ДВИЖЕНИЕ К ТОЧКЕ ---
    private void MoveToPoint(int index)
    {
        if (!isPatrolling || patrolPoints.Length == 0) return;
        if (isChasing) return;

        Transform targetPoint = patrolPoints[index];

        entity.SetData(new CommandRequest
        {
            type = CommandType.MOVE_TO_POSITION,
            args = targetPoint.position,
            status = CommandStatus.IDLE
        });

        // Debug.Log("Враг идёт к точке!");
    }

    // --- ТРИГГЕР: ИГРОК ВОШЁЛ В ЗОНУ ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ally"))
        {
            playerEntity = other.GetComponent<Entity>();
            if (playerEntity != null)
            {
                isChasing = true;
                isPatrolling = false;
                AttackPlayer();
            }
        }
    }

    // --- ТРИГГЕР: ИГРОК ВЫШЕЛ ИЗ ЗОНЫ ---
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ally"))
        {
            isChasing = false;
            isPatrolling = true;
            MoveToPoint(currentPointIndex);
        }
    }

    // --- АТАКА ИГРОКА ---
    private void AttackPlayer()
    {
        if (playerEntity != null && isChasing)
        {
            if (playerEntity.HasData<HitPointsComponent>())
            {
                entity.SetData(new CommandRequest
                {
                    type = CommandType.ATTACK_TARGET,
                    args = playerEntity,
                    status = CommandStatus.IDLE
                });
            }
            else
            {
                isChasing = false;
                isPatrolling = true;
                MoveToPoint(currentPointIndex);
            }
        }
    }

    public void StopPatrolling()
    {
        isPatrolling = false;
        isChasing = false;
        StopAttack();
    }

    public void StartPatrolling()
    {
        isPatrolling = true;
        MoveToPoint(currentPointIndex);
    }

    private void StopAttack()
    {
        entity.RemoveData<CommandRequest>();
    }
}