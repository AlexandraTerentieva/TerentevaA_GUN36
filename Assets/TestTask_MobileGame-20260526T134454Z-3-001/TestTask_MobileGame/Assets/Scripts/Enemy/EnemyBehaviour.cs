using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Поведение врага: Idle → обнаружение игрока → преследование → атака.
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Движение")]
    public float moveSpeed = 3f;          // Скорость передвижения врага
    public float stoppingDistance = 2f;   // Дистанция, на которой враг останавливается для атаки

    [Header("Зона видимости")]
    public float detectionRadius = 10f;   // Радиус, в котором враг замечает игрока

    [Header("Проверка земли")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayerMask;

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private Rigidbody rb;                 // Ссылка на физическое тело
    private EnemyEntity entity;           // Ссылка на характеристики врага
    private bool isGrounded;              // На земле ли враг
    private Vector3 targetVelocity;       // Целевая скорость для движения
    private CapsuleCollider capsuleCollider;
    private Transform player;             // Ссылка на игрока
    private bool isPlayerDetected = false;
    private Animator animator;            // Для анимаций

    // ============================================================
    //  СТАРТ
    // ============================================================

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        entity = GetComponent<EnemyEntity>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        // Находим игрока по тегу
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log($"✅ {name}: Игрок найден!");
        }
        else
        {
            Debug.LogWarning($"⚠️ {name}: Игрок не найден! Добавь тег 'Player'.");
        }

        // Настройка Rigidbody
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        if (groundLayerMask == 0)
        {
            groundLayerMask = LayerMask.GetMask("Default");
        }

        Debug.Log($"✅ {name}: EnemyBehaviour инициализирован");
    }

    // ============================================================
    //  ОБНОВЛЕНИЕ (каждый кадр)
    // ============================================================

    void Update()
    {
        // Если враг мёртв — ничего не делаем
        if (entity != null && entity.IsDead())
        {
            targetVelocity = Vector3.zero;
            PlayIdleAnimation();
            return;
        }

        if (player == null)
        {
            targetVelocity = Vector3.zero;
            PlayIdleAnimation();
            return;
        }

        // ============================================================
        //  ПРОВЕРКА ЗЕМЛИ (Raycast)
        // ============================================================

        float capsuleHeight = capsuleCollider != null ? capsuleCollider.height : 1.8f;
        Vector3 rayOrigin = transform.position + Vector3.up * (capsuleHeight * 0.5f);
        float rayLength = capsuleHeight * 0.5f + groundCheckDistance;

        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayLength, groundLayerMask);
        Debug.DrawRay(rayOrigin, Vector3.down * rayLength, isGrounded ? Color.green : Color.red);

        // ============================================================
        //  ОБНАРУЖЕНИЕ ИГРОКА
        // ============================================================

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerDetected = distanceToPlayer <= detectionRadius;

        // ============================================================
        //  ПОВЕДЕНИЕ: если игрок в зоне видимости
        // ============================================================

        if (isPlayerDetected)
        {
            Vector3 directionToPlayer = (player.position - transform.position);
            directionToPlayer.y = 0; // Игнорируем высоту

            float horizontalDistance = directionToPlayer.magnitude;

            // Если игрок далеко — идём к нему
            if (horizontalDistance > stoppingDistance)
            {
                Vector3 moveDirection = directionToPlayer.normalized;

                if (moveDirection != Vector3.zero)
                {
                    // Поворачиваемся в сторону игрока
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);

                    // Двигаемся
                    targetVelocity = moveDirection * moveSpeed;
                }

                PlayWalkAnimation();
            }
            else
            {
                // Игрок рядом — стоим и атакуем
                targetVelocity = Vector3.zero;

                if (entity != null && horizontalDistance <= entity.attackRange)
                {
                    CharacterEntity playerEntity = player.GetComponent<CharacterEntity>();
                    if (playerEntity != null && !playerEntity.IsDead())
                    {
                        entity.Attack(playerEntity);
                        PlayAttackAnimation();
                    }
                }
                else
                {
                    PlayIdleAnimation();
                }
            }
        }
        else
        {
            // ============================================================
            //  РЕЖИМ IDLE: игрок вне зоны видимости
            // ============================================================

            targetVelocity = Vector3.zero;
            PlayIdleAnimation();
        }
    }

    // ============================================================
    //  FIXEDUPDATE (физика)
    // ============================================================

    void FixedUpdate()
    {
        // Если враг мёртв — не двигаем его
        if (entity != null && entity.IsDead())
        {
            return;
        }

        // Применяем движение через физику
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    // ============================================================
    //  АНИМАЦИИ
    // ============================================================

    private void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsAttacking", false);
            animator.SetTrigger("Idle");
        }
    }

    private void PlayWalkAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", false);
        }
    }

    private void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger("Attack");
        }
    }

    // ============================================================
    //  ВИЗУАЛИЗАЦИЯ В СЦЕНЕ (Gizmos)
    // ============================================================

    void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;

        float capsuleHeight = capsuleCollider.height;
        Vector3 rayOrigin = transform.position + Vector3.up * (capsuleHeight * 0.5f);
        float rayLength = capsuleHeight * 0.5f + groundCheckDistance;

        // Луч проверки земли
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(rayOrigin - Vector3.up * rayLength, 0.1f);
        Gizmos.DrawLine(rayOrigin, rayOrigin - Vector3.up * rayLength);

        // Зона видимости (синяя)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Радиус атаки (красная)
        if (entity != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, entity.attackRange);
        }

        // Дистанция остановки (жёлтая)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}