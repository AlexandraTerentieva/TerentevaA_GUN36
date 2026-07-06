using UnityEngine;

public class AiReturnState : AiState
{
    public AiStateId GetId() => AiStateId.Return;

    public void Enter(AiAgent agent)
    {
        if (agent.returnPoint != null)
            agent.agent.SetDestination(agent.returnPoint.position);
    }

    public void Update(AiAgent agent)
    {
        if (agent.returnPoint == null) return;

        float dist = Vector3.Distance(agent.transform.position, agent.returnPoint.position);
        if (dist <= 1.5f)
        {
            agent.collectedCount = 0;
            agent.UpdateScoreUI();
            agent.stateMachine.ChangeState(AiStateId.Idle);
        }
    }

    public void Exit(AiAgent agent) { }
}