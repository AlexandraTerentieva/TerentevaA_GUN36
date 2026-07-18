using UnityEngine;

public class AiMeleeAttackState : AiState
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float timer = 0f;

    public AiStateId GetId()
    {
        return AiStateId.AttackTarget;
    }

    public void Enter(AiAgent agent)
    {
        timer = 0f;
        agent.navMeshAgent.stoppingDistance = attackRange;
        Debug.Log("[Melee] Враг перешёл в режим ближнего боя");
    }

    public void Update(AiAgent agent)
    {
        if (agent.health.IsDead())
        {
            agent.stateMachine.ChangeState(AiStateId.Death);
            return;
        }

        if (agent.playerTransform == null)
        {
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }

        if (agent.health.IsLowHealth())
        {
            agent.stateMachine.ChangeState(AiStateId.FindHealth);
            return;
        }

        Transform target = agent.playerTransform;
        agent.navMeshAgent.destination = target.position;

        float distance = Vector3.Distance(agent.transform.position, target.position);

        if (distance <= attackRange)
        {
            agent.navMeshAgent.ResetPath();
            timer += Time.deltaTime;

            if (timer >= attackCooldown)
            {
                timer = 0f;

                Animator animator = agent.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play("Attack", 0, 0f);
                    Debug.Log("[Melee] Анимация удара проиграна");
                }

                // 👇 ИСПРАВЛЕННАЯ СТРОКА
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(10);
                    Debug.Log("[Melee] Враг ударил игрока! Урон нанесён!");
                }
                else
                {
                    Debug.LogWarning("[Melee] У игрока нет компонента PlayerHealth!");
                }
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.ResetPath();
    }
}