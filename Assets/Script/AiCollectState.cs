using UnityEngine;

public class AiCollectState : AiState
{
    public AiStateId GetId() => AiStateId.Collect;

    public void Enter(AiAgent agent)
    {
        if (agent.targetCollectible != null)
            agent.agent.SetDestination(agent.targetCollectible.transform.position);
    }

    public void Update(AiAgent agent)
    {
        if (agent.targetCollectible == null || !agent.targetCollectible.activeSelf)
        {
            agent.stateMachine.ChangeState(AiStateId.Search);
            return;
        }

        float dist = Vector3.Distance(agent.transform.position, agent.targetCollectible.transform.position);
        if (dist <= 1.5f)
        {
            agent.targetCollectible.SetActive(false);
            agent.collectedCount++;
            agent.UpdateScoreUI();

            if (agent.collectedCount >= agent.maxCollectibles)
                agent.stateMachine.ChangeState(AiStateId.Return);
            else
                agent.stateMachine.ChangeState(AiStateId.Search);
        }
    }

    public void Exit(AiAgent agent) { }
}