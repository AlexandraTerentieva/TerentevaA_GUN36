using UnityEngine;

public class AiAttackTargetState : AiState
{
    public AiStateId GetId() => AiStateId.AttackTarget;

    public void Enter(AiAgent agent)
    {
        agent.weapons?.ActivateWeapon();
        agent.navMeshAgent.stoppingDistance = agent.config.attackStoppingDistance;
        agent.navMeshAgent.speed = agent.config.attackSpeed;
    }

    public void Update(AiAgent agent)
    {
        // Если игрок пропал — ищем снова
        if (agent.playerTransform == null)
        {
            agent.weapons.SetFiring(false);
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }

        // Целимся и стреляем
        agent.weapons.SetTarget(agent.playerTransform);
        agent.navMeshAgent.destination = agent.playerTransform.position;
        agent.weapons.SetFiring(true);

        // ПРОВЕРКА ПАТРОНОВ (принудительная)
        if (agent.weapons != null && agent.weapons.currentWeapon != null)
        {
            // Если патроны кончились — ищем новые
            if (agent.weapons.currentWeapon.clipCount <= 0)
            {
                Debug.Log("Патроны кончились! Ищу боезапас.");
                agent.weapons.SetFiring(false);
                agent.stateMachine.ChangeState(AiStateId.FindAmmo);
                return;
            }
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.weapons?.DeactivateWeapon();
        agent.navMeshAgent.stoppingDistance = 0f;
    }
}