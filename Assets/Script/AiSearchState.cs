using UnityEngine;
using UnityEngine.AI;

public class AiSearchState : AiState
{
    public AiStateId GetId() => AiStateId.Search;

    public void Enter(AiAgent agent)
    {
        SetRandomDestination(agent);
    }

    public void Update(AiAgent agent)
    {
        if (!agent.agent.hasPath && !agent.agent.pathPending)
        {
            SetRandomDestination(agent);
        }

        GameObject nearest = GetNearestCollectible(agent);
        if (nearest != null && Vector3.Distance(agent.transform.position, nearest.transform.position) <= agent.collectRadius)
        {
            agent.targetCollectible = nearest;
            agent.stateMachine.ChangeState(AiStateId.Collect);
        }
    }

    public void Exit(AiAgent agent) { }

    private void SetRandomDestination(AiAgent agent)
    {
        Vector3 randomPos = Random.insideUnitSphere * 10f + agent.transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.agent.SetDestination(hit.position);
        }
    }

    private GameObject GetNearestCollectible(AiAgent agent)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Collectible");
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var item in items)
        {
            if (item == null || !item.activeSelf) continue;
            float dist = Vector3.Distance(agent.transform.position, item.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = item;
            }
        }
        return nearest;
    }
}