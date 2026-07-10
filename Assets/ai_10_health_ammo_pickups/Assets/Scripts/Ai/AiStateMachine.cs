using System.Collections.Generic;
using UnityEngine;

public class AiStateMachine
{
    private AiAgent agent;
    private AiState currentState;
    private Dictionary<AiStateId, AiState> states = new Dictionary<AiStateId, AiState>();

    public AiStateMachine(AiAgent agent)
    {
        this.agent = agent;
    }

    public void RegisterState(AiState state)
    {
        states[state.GetId()] = state;
    }

    public AiState GetState(AiStateId stateId)
    {
        return states.TryGetValue(stateId, out AiState state) ? state : null;
    }

    public void ChangeState(AiStateId newStateId)
    {
        if (currentState != null)
            currentState.Exit(agent);

        if (states.TryGetValue(newStateId, out AiState state))
        {
            currentState = state;
            currentState.Enter(agent);
        }
        else
        {
            Debug.LogError($"State {newStateId} not registered!");
        }
    }

    public void Update()
    {
        currentState?.Update(agent);
    }
}