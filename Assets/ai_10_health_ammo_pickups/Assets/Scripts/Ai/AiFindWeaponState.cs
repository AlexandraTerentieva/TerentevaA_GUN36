using UnityEngine;

public class AiFindWeaponState : AiState
{
    public AiStateId GetId() => AiStateId.FindWeapon;

    public void Enter(AiAgent agent)
    {
        GameObject pickup = FindClosestWeapon(agent);
        if (pickup != null)
        {
            agent.navMeshAgent.destination = pickup.transform.position;
            agent.navMeshAgent.speed = 5f;
        }
        else
        {
            SetRandomDestination(agent);
        }
    }

    public void Update(AiAgent agent)
    {
        if (agent.weapons.HasWeapon())
        {
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }

        if (!agent.navMeshAgent.hasPath && !agent.navMeshAgent.pathPending)
        {
            SetRandomDestination(agent);
        }
    }

    public void Exit(AiAgent agent) { }

    private GameObject FindClosestWeapon(AiAgent agent)
    {
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Weapon");
        if (pickups.Length == 0) return null;

        GameObject closest = pickups[0];
        float minDist = Vector3.Distance(agent.transform.position, closest.transform.position);

        foreach (var pickup in pickups)
        {
            float dist = Vector3.Distance(agent.transform.position, pickup.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = pickup;
            }
        }
        return closest;
    }

    private void SetRandomDestination(AiAgent agent)
    {
        Vector3 randomPos = Random.insideUnitSphere * 10f + agent.transform.position;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPos, out UnityEngine.AI.NavMeshHit hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.navMeshAgent.destination = hit.position;
        }
    }
}