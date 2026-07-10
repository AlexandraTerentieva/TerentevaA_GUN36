using UnityEngine;

public class AiFindAmmoState : AiState
{
    private GameObject targetPickup;

    public AiStateId GetId() => AiStateId.FindAmmo;

    public void Enter(AiAgent agent)
    {
        targetPickup = null;
        agent.navMeshAgent.speed = agent.config.findWeaponSpeed;
        agent.navMeshAgent.ResetPath();
        Debug.Log("Вошёл в поиск патронов!");
    }

    public void Update(AiAgent agent)
    {
        if (targetPickup == null)
        {
            targetPickup = FindClosestAmmo(agent);
            if (targetPickup != null)
            {
                agent.navMeshAgent.destination = targetPickup.transform.position;
                Debug.Log("Нашёл патроны! Иду к ним.");
            }
            else
            {
                Debug.Log("Патронов нет на сцене!");
                SetRandomDestination(agent);
            }
        }

        if (!agent.navMeshAgent.hasPath && !agent.navMeshAgent.pathPending)
        {
            SetRandomDestination(agent);
        }

        if (targetPickup != null && Vector3.Distance(agent.transform.position, targetPickup.transform.position) < 1.5f)
        {
            Debug.Log("Подобрал патроны!");
            agent.weapons.RefillAmmo(30);
            targetPickup.SetActive(false);
            targetPickup = null;
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
    }

    public void Exit(AiAgent agent) { }

    private GameObject FindClosestAmmo(AiAgent agent)
    {
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Ammo");
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