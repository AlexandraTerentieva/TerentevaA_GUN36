using UnityEngine;
using Game.GameEngine.Ecs;
using SampleProject;

public class EnemyPatrol : MonoBehaviour
{
    // --- ОБЪЯВЛЯЕМ ПЕРЕМЕННЫЕ (все нужны для патрулирования и атаки) ---
    private Entity entity;                // Entity врага (чтобы давать ему команды)
    private Transform[] patrolPoints;     // Массив точек патрулирования
    private int currentPointIndex = 0;    // Индекс текущей точки (с какой начинаем)
    private bool isPatrolling = true;     // Патрулируем? (true = да)
    private float reachDistance = 1.5f;   // Расстояние до точки, чтобы считать, что дошли
    private bool isWaiting = false;       // Стоим на точке и ждём?
    private float waitTime = 4f;          // Сколько секунд стоим на точке
    private float waitTimer = 0f;         // Таймер ожидания (сколько осталось)

    private bool isChasing = false;       // Преследуем ли игрока?
    private Entity playerEntity;          // Entity игрока (цель для атаки)

    private void Start()
    {
        // Находим Entity врага
        entity = GetComponent<Entity>();

        // Ищем все объекты, в названии которых есть "PatrolPoint"
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        System.Collections.Generic.List<Transform> pointsList = new System.Collections.Generic.List<Transform>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("PatrolPoint"))
            {
                pointsList.Add(obj.transform);
            }
        }

        // Превращаем список в массив
        patrolPoints = pointsList.ToArray();

        // Если точки есть — начинаем патрулирование
        if (patrolPoints.Length > 0)
        {
            MoveToPoint(currentPointIndex);
        }
    }

    private void Update()
    {
        // Если патрулирование выключено, нет точек или мы атакуем — выходим
        if (!isPatrolling || patrolPoints.Length == 0) return;
        if (isChasing) return; // Если атакуем — патрулирование отключено

        // Если мы ждём на точке — уменьшаем таймер
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                // Время вышло — переключаемся на следующую точку
                isWaiting = false;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                MoveToPoint(currentPointIndex);
            }
            return;
        }

        // Проверяем расстояние до текущей точки
        Transform targetPoint = patrolPoints[currentPointIndex];
        float distance = Vector3.Distance(transform.position, targetPoint.position);

        // Если подошли достаточно близко — начинаем ждать
        if (distance < reachDistance)
        {
            isWaiting = true;
            waitTimer = waitTime;
            Debug.Log($"Враг стоит на точке {currentPointIndex} {waitTime} секунд");
        }
    }

    // --- ДВИЖЕНИЕ К ТОЧКЕ ---
    private void MoveToPoint(int index)
    {
        if (!isPatrolling || patrolPoints.Length == 0) return;
        if (isChasing) return;

        Transform targetPoint = patrolPoints[index];

        // Отправляем команду напрямую в Entity врага
        entity.SetData(new CommandRequest
        {
            type = CommandType.MOVE_TO_POSITION,
            args = targetPoint.position,
            status = CommandStatus.IDLE
        });

        Debug.Log($"Враг идёт к точке {index}: {targetPoint.name}");
    }

    // --- ТРИГГЕРЫ ДЛЯ ОБНАРУЖЕНИЯ ИГРОКА ---
    // Срабатывает, когда игрок входит в зону видимости (триггер на враге)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntity = other.GetComponent<Entity>();
            if (playerEntity != null)
            {
                // Переключаем врага в режим преследования
                isChasing = true;
                isPatrolling = false;
                AttackPlayer();
                Debug.Log("Враг заметил игрока! Атакуем!");
            }
        }
    }

    // Срабатывает, когда игрок выходит из зоны видимости
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Возвращаем врага в режим патрулирования
            isChasing = false;
            isPatrolling = true;
            MoveToPoint(currentPointIndex);
            Debug.Log("Игрок вышел из зоны. Возвращаемся к патрулированию.");
        }
    }

    // --- ЛОГИКА АТАКИ ---
    // Атакует игрока
    private void AttackPlayer()
    {
        if (playerEntity != null && isChasing)
        {
            // Проверяем, есть ли у игрока компонент здоровья (чтобы не было ошибки)
            if (playerEntity.HasData<HitPointsComponent>())
            {
                // Отправляем команду атаки в Entity врага
                entity.SetData(new CommandRequest
                {
                    type = CommandType.ATTACK_TARGET,
                    args = playerEntity,
                    status = CommandStatus.IDLE
                });

                // Проверяем каждые 2 секунды, жив ли игрок
                Invoke(nameof(CheckPlayerAlive), 2f);
            }
            else
            {
                // Если у игрока нет здоровья — выходим из режима атаки
                Debug.LogWarning("У игрока нет компонента HitPointsComponent!");
                isChasing = false;
                isPatrolling = true;
                MoveToPoint(currentPointIndex);
            }
        }
    }

    // Проверяет, жив ли игрок
    private void CheckPlayerAlive()
    {
        if (!isChasing) return;

        // Если игрок уничтожен — возвращаемся к патрулированию
        if (playerEntity == null)
        {
            isChasing = false;
            isPatrolling = true;
            MoveToPoint(currentPointIndex);
            Debug.Log("Игрок уничтожен. Возвращаемся к патрулированию.");
            return;
        }

        // Если у игрока нет компонента здоровья — возвращаемся к патрулированию
        if (!playerEntity.HasData<HitPointsComponent>())
        {
            isChasing = false;
            isPatrolling = true;
            MoveToPoint(currentPointIndex);
            Debug.Log("У игрока нет здоровья. Возвращаемся к патрулированию.");
            return;
        }

        // Получаем здоровье игрока
        HitPointsComponent hp = playerEntity.GetData<HitPointsComponent>();

        // Если здоровье игрока <= 0 — он мёртв
        if (hp.current <= 0)
        {
            isChasing = false;
            isPatrolling = true;
            MoveToPoint(currentPointIndex);
            Debug.Log("Игрок мёртв. Возвращаемся к патрулированию.");
        }
        else
        {
            // Продолжаем атаковать
            AttackPlayer();
        }
    }

    // --- ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ ---
    // Останавливает патрулирование
    public void StopPatrolling()
    {
        isPatrolling = false;
    }

    // Запускает патрулирование
    public void StartPatrolling()
    {
        isPatrolling = true;
        MoveToPoint(currentPointIndex);
    }
}