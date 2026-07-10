using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AiState
{
    private float timer = 0f;

    public AiStateId GetId() => AiStateId.ChasePlayer;

    public void Enter(AiAgent agent)
    {
        timer = agent.config.maxTime;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.enabled || agent.playerTransform == null) return;

        timer -= Time.deltaTime;

        if (!agent.navMeshAgent.hasPath && !agent.navMeshAgent.pathPending)
        {
            agent.navMeshAgent.destination = agent.playerTransform.position;
        }

        if (timer <= 0f)
        {
            Vector3 direction = agent.playerTransform.position - agent.navMeshAgent.destination;
            direction.y = 0;

            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance ||
                agent.navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                agent.navMeshAgent.destination = agent.playerTransform.position;
            }

            timer = agent.config.maxTime;
        }
    }

    public void Exit(AiAgent agent) { }
}