using UnityEngine;

public class AiIdleState : AiState
{
    private float timer;

    public AiStateId GetId() => AiStateId.Idle;

    public void Enter(AiAgent agent)
    {
        timer = agent.idleDuration;
        agent.agent.ResetPath();
    }

    public void Update(AiAgent agent)
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            agent.stateMachine.ChangeState(AiStateId.Search);
        }
    }

    public void Exit(AiAgent agent) { }
}