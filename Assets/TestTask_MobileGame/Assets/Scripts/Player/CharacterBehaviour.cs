using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управление игроком: движение, прыжок, атака, сбор и сдача ресурсов.
/// Прикрепляется к объекту игрока.
/// </summary>
public class CharacterBehaviour : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Движение")]
    public float moveSpeed = 5f;            // Скорость передвижения игрока

    [Header("Прыжок")]
    public float jumpForce = 7f;            // Сила прыжка
    public float groundCheckDistance = 0.3f; // Длина луча для проверки земли

    [Header("Проверка земли")]
    public LayerMask groundLayerMask;        // Слой, который считается землёй

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private Rigidbody rb;                   // Ссылка на компонент Rigidbody
    private CharacterEntity entity;         // Ссылка на компонент CharacterEntity (здоровье игрока)
    private bool isGrounded;                // Находится ли игрок на земле
    private Vector3 targetVelocity;         // Целевая скорость (для движения)
    private CapsuleCollider capsuleCollider; // Ссылка на компонент CapsuleCollider
    private Camera mainCamera;              // Ссылка на главную камеру

    // Хранилище собранных ресурсов (инвентарь юнита)
    private int woodInventory = 0;          // Количество древесины в инвентаре
    private int stoneInventory = 0;         // Количество камня в инвентаре
    private int goldInventory = 0;          // Количество золота в инвентаре

    // ============================================================
    //  СТАРТ
    //  Вызывается при запуске игры
    // ============================================================

    void Start()
    {
        // Получаем все необходимые компоненты
        rb = GetComponent<Rigidbody>();
        entity = GetComponent<CharacterEntity>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;

        // Настройка Rigidbody для правильной физики
        if (rb != null)
        {
            // Замораживаем вращение по осям X и Z (чтобы персонаж не падал на бок)
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            // Плавная интерполяция для более гладкого движения
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // Если слой земли не назначен в инспекторе — используем слой Default
        if (groundLayerMask == 0)
        {
            groundLayerMask = LayerMask.GetMask("Default");
        }

        Debug.Log($"✅ {name}: CharacterBehaviour инициализирован");
    }

    // ============================================================
    //  ОБНОВЛЕНИЕ
    //  Вызывается каждый кадр. Здесь обрабатываем ввод.
    // ============================================================

    void Update()
    {
        // Если игрок мёртв — отключаем управление
        if (entity != null && entity.IsDead())
        {
            targetVelocity = Vector3.zero;
            return;
        }

        // ============================================================
        //  ПРОВЕРКА ЗЕМЛИ
        //  Пускаем луч от нижней части капсулы вниз.
        //  Если луч касается земли — игрок на земле.
        // ============================================================

        // Получаем высоту капсулы
        float capsuleHeight = capsuleCollider != null ? capsuleCollider.height : 1.8f;

        // Точка старта луча — от центра капсулы
        Vector3 rayOrigin = transform.position + Vector3.up * (capsuleHeight * 0.5f);

        // Длина луча = половина высоты капсулы + запас
        float rayLength = capsuleHeight * 0.5f + groundCheckDistance;

        // Пускаем луч и проверяем, касается ли он слоя земли
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayLength, groundLayerMask);

        // Визуализация луча в Scene View (зелёный — на земле, красный — в воздухе)
        Debug.DrawRay(rayOrigin, Vector3.down * rayLength, isGrounded ? Color.green : Color.red);

        // ============================================================
        //  ДВИЖЕНИЕ (WASD)
        //  Получаем ввод с клавиатуры и двигаем игрока
        // ============================================================

        float horizontal = Input.GetAxis("Horizontal"); // A / D
        float vertical = Input.GetAxis("Vertical");     // W / S

        // Получаем направление движения относительно камеры
        Vector3 movement = GetCameraRelativeMovement(horizontal, vertical);

        // Если есть движение — поворачиваем игрока и задаём скорость
        if (movement != Vector3.zero)
        {
            // Плавный поворот в сторону движения
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            // Сохраняем целевую скорость
            targetVelocity = movement * moveSpeed;
        }
        else
        {
            // Если игрок не двигается — скорость равна нулю
            targetVelocity = Vector3.zero;
        }

        // ============================================================
        //  ПРЫЖОК (Пробел)
        //  Игрок может прыгнуть только если стоит на земле
        // ============================================================

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Сбрасываем вертикальную скорость перед прыжком
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            // Применяем силу прыжка
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Снимаем флаг земли (чтобы нельзя было прыгнуть второй раз в воздухе)
            isGrounded = false;

            Debug.Log("🚀 ПРЫЖОК!");
        }

        // ============================================================
        //  АТАКА (Левая кнопка мыши)
        //  Игрок атакует врага с компонентом EnemyEntity
        // ============================================================

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("⚔️ АТАКА!");

            // Находим все объекты в радиусе атаки
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, entity.attackRange);

            // Проверяем каждый объект
            foreach (var hit in hitColliders)
            {
                // Ищем компонент EnemyEntity (только у врагов)
                EnemyEntity enemyTarget = hit.GetComponent<EnemyEntity>();
                if (enemyTarget != null && !enemyTarget.IsDead())
                {
                    // Наносим урон врагу
                    enemyTarget.TakeDamage(entity.attackDamage);
                    Debug.Log($"⚔️ Игрок атаковал врага {enemyTarget.name} на {entity.attackDamage} урона!");
                    break; // Атакуем только одного врага за раз
                }
            }
        }

        // ============================================================
        //  СБОР РЕСУРСОВ (нажатие E)
        //  Игрок собирает ресурс с холма и кладёт в инвентарь
        // ============================================================

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Находим все объекты в радиусе сбора (2 метра)
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);

            foreach (var hit in hitColliders)
            {
                // Проверяем, есть ли холм с ресурсами
                ResourceHill hill = hit.GetComponent<ResourceHill>();
                if (hill != null && hill.HasResource())
                {
                    // Собираем ресурс с холма
                    int collected = hill.CollectResource();

                    // Кладём ресурс в инвентарь юнита (в зависимости от типа)
                    switch (hill.resourceType)
                    {
                        case "Wood":
                            woodInventory += collected;
                            break;
                        case "Stone":
                            stoneInventory += collected;
                            break;
                        case "Gold":
                            goldInventory += collected;
                            break;
                    }

                    // Выводим информацию о ресурсах в инвентаре
                    Debug.Log($"✅ {name} собрал {collected} {hill.resourceType}. В инвентаре: Древесина={woodInventory}, Камень={stoneInventory}, Золото={goldInventory}");
                    break; // Собираем только один ресурс за раз
                }
            }
        }

        // ============================================================
        //  СДАЧА РЕСУРСОВ НА БАЗУ (нажатие Q)
        //  Игрок подходит к базе и сдаёт все ресурсы из инвентаря
        // ============================================================

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Проверяем, есть ли база рядом (радиус 3 метра)
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
            BaseEntity baseEntity = null;

            // Ищем базу среди объектов вокруг
            foreach (var hit in hitColliders)
            {
                baseEntity = hit.GetComponent<BaseEntity>();
                if (baseEntity != null) break;
            }

            // Если базы нет рядом — сообщаем игроку
            if (baseEntity == null)
            {
                Debug.Log("⚠️ Рядом нет базы! Подойди к базе и нажми Q.");
                return;
            }

            // Проверяем, есть ли что сдавать
            int totalResources = woodInventory + stoneInventory + goldInventory;
            if (totalResources == 0)
            {
                Debug.Log("⚠️ У тебя нет ресурсов для сдачи! Сначала собери их (E).");
                return;
            }

            // Сдаём ресурсы на базу (по очереди)
            if (woodInventory > 0)
            {
                baseEntity.AddResource("Wood", woodInventory);
                Debug.Log($"🪵 Сдано {woodInventory} древесины на базу!");
                woodInventory = 0; // Очищаем инвентарь
            }

            if (stoneInventory > 0)
            {
                baseEntity.AddResource("Stone", stoneInventory);
                Debug.Log($"🪨 Сдано {stoneInventory} камня на базу!");
                stoneInventory = 0;
            }

            if (goldInventory > 0)
            {
                baseEntity.AddResource("Gold", goldInventory);
                Debug.Log($"✨ Сдано {goldInventory} золота на базу!");
                goldInventory = 0;
            }

            Debug.Log($"✅ Все ресурсы сданы на базу!");
        }
    }

    // ============================================================
    //  FIXEDUPDATE
    //  Вызывается с фиксированной частотой. Здесь применяем физику.
    // ============================================================

    void FixedUpdate()
    {
        // Если игрок мёртв — не двигаем его (чтобы не было ошибок с кинематическим телом)
        if (entity != null && entity.IsDead())
        {
            return;
        }

        // Применяем движение через физику (чтобы не было дёрганий)
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    // ============================================================
    //  МЕТОД: движение относительно камеры
    //  Преобразует ввод с клавиатуры в направление относительно камеры.
    // ============================================================

    private Vector3 GetCameraRelativeMovement(float horizontal, float vertical)
    {
        // Если камера не найдена — используем глобальные оси
        if (mainCamera == null)
        {
            return new Vector3(horizontal, 0, vertical).normalized;
        }

        // Получаем направления камеры
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        // Обнуляем Y, чтобы персонаж не летел вверх/вниз
        forward.y = 0;
        right.y = 0;

        // Нормализуем направления (чтобы длина была равна 1)
        forward.Normalize();
        right.Normalize();

        // Суммируем направления с учётом ввода
        return (forward * vertical + right * horizontal).normalized;
    }

    // ============================================================
    //  ВИЗУАЛИЗАЦИЯ (для отладки в Scene View)
    //  Рисует луч проверки земли и радиус атаки
    // ============================================================

    void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;

        // Рисуем луч проверки земли
        float capsuleHeight = capsuleCollider.height;
        Vector3 rayOrigin = transform.position + Vector3.up * (capsuleHeight * 0.5f);
        float rayLength = capsuleHeight * 0.5f + groundCheckDistance;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(rayOrigin - Vector3.up * rayLength, 0.1f);
        Gizmos.DrawLine(rayOrigin, rayOrigin - Vector3.up * rayLength);

        // Рисуем радиус атаки (красный круг)
        if (entity != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, entity.attackRange);
        }
    }
}