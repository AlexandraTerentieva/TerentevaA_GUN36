using UnityEngine;

public class AiIdleState : AiState
{
    public AiStateId GetId() => AiStateId.Idle;

    public void Enter(AiAgent agent)
    {
        agent.weapons?.DeactivateWeapon();
        agent.navMeshAgent.ResetPath();
    }

    public void Update(AiAgent agent)
    {
        if (agent.playerTransform == null) return;

        Health playerHealth = agent.playerTransform.GetComponent<Health>();
        if (playerHealth != null && playerHealth.IsDead()) return;

        float distance = Vector3.Distance(agent.transform.position, agent.playerTransform.position);
        if (distance > agent.config.maxSightDistance) return;

        Vector3 directionToPlayer = (agent.playerTransform.position - agent.transform.position).normalized;
        float dot = Vector3.Dot(agent.transform.forward, directionToPlayer);

        if (dot > 0f)
        {
            agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
        }
    }

    public void Exit(AiAgent agent) { }
}