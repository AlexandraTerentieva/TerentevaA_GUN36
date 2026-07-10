using UnityEngine;

public class AiFindTargetState : AiState
{
    public AiStateId GetId() => AiStateId.FindTarget;

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.findTargetSpeed;

        // Находим игрока и сразу назначаем его целью
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            agent.playerTransform = player.transform;
            agent.stateMachine.ChangeState(AiStateId.AttackTarget);
            Debug.Log("Цель назначена вручную!");
        }
    }

    public void Update(AiAgent agent) { }

    public void Exit(AiAgent agent) { }
}