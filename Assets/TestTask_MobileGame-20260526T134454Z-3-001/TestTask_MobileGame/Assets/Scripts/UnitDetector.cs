using UnityEngine;

/// <summary>
/// Обнаружение врага и переход в режим атаки.
/// Юниты стоят на месте (Idle), но когда враг входит в зону видимости —
/// все юниты бегут к нему и атакуют.
/// </summary>
public class UnitDetector : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Зона видимости")]
    public float detectionRadius = 10f;    // Радиус, в котором юнит замечает врага

    [Header("Движение")]
    public float moveSpeed = 5f;           // Скорость бега к врагу

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private Transform enemyTarget;         // Ссылка на врага
    private bool isEnemyDetected = false;  // Обнаружен ли враг
    private Animator animator;             // Ссылка на Animator (для анимаций)

    // ============================================================
    //  СТАРТ
    //  Вызывается при запуске игры
    // ============================================================

    void Start()
    {
        animator = GetComponent<Animator>();

        // Ищем врага по имени "Enemy"
        GameObject enemy = GameObject.Find("Enemy");
        if (enemy != null)
        {
            enemyTarget = enemy.transform;
            Debug.Log($"✅ {name}: Враг найден по имени 'Enemy'!");
        }
        else
        {
            Debug.LogWarning($"⚠️ {name}: Враг с именем 'Enemy' не найден!");
        }
    }

    // ============================================================
    //  ОБНОВЛЕНИЕ
    //  Вызывается каждый кадр
    // ============================================================

    void Update()
    {
        // Если врага нет — выходим
        if (enemyTarget == null) return;

        // Проверяем расстояние до врага
        float distance = Vector3.Distance(transform.position, enemyTarget.position);

        // Если враг в зоне видимости — атакуем
        if (distance <= detectionRadius)
        {
            isEnemyDetected = true;
            AttackEnemy();
        }
        else
        {
            isEnemyDetected = false;
            // Стоим на месте (Idle)
            PlayIdleAnimation();
        }
    }

    // ============================================================
    //  АТАКА ВРАГА
    // ============================================================

    private void AttackEnemy()
    {
        // Направление к врагу
        Vector3 direction = (enemyTarget.position - transform.position);
        direction.y = 0; // Игнорируем высоту

        float distanceToEnemy = direction.magnitude;

        // Поворачиваемся в сторону врага
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }

        // Если враг далеко — идём к нему
        if (distanceToEnemy > 2f)
        {
            // Двигаемся к врагу
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            PlayWalkAnimation();
        }
        else
        {
            // Враг рядом — атакуем
            EnemyEntity enemy = enemyTarget.GetComponent<EnemyEntity>();
            if (enemy != null && !enemy.IsDead())
            {
                CharacterEntity playerEntity = GetComponent<CharacterEntity>();
                if (playerEntity != null)
                {
                    enemy.TakeDamage(playerEntity.attackDamage);
                    Debug.Log($"⚔️ {name} атаковал врага на {playerEntity.attackDamage} урона!");
                    PlayAttackAnimation();
                }
            }
        }
    }

    // ============================================================
    //  АНИМАЦИИ
    // ============================================================

    /// <summary>
    /// Проигрывает анимацию Idle (стоять на месте)
    /// </summary>
    private void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsAttacking", false);
            animator.SetTrigger("Idle");
        }
    }

    /// <summary>
    /// Проигрывает анимацию ходьбы
    /// </summary>
    private void PlayWalkAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", false);
        }
    }

    /// <summary>
    /// Проигрывает анимацию атаки
    /// </summary>
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
    //  ВИЗУАЛИЗАЦИЯ (для отладки в Scene View)
    // ============================================================

    void OnDrawGizmosSelected()
    {
        // Зона видимости (синий круг)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Если враг найден — рисуем линию к нему
        if (enemyTarget != null && isEnemyDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, enemyTarget.position);
        }
    }
}