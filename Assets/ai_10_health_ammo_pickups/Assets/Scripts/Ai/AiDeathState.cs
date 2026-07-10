using UnityEngine;

public class AiDeathState : AiState
{
    public Vector3 direction;

    public AiStateId GetId() => AiStateId.Death;

    public void Enter(AiAgent agent)
    {
        if (agent.ragdoll != null)
        {
            agent.ragdoll.ActivateRagdoll();
            agent.ragdoll.ApplyForce(direction * agent.config.dieForce);
        }

        if (agent.ui != null)
            agent.ui.gameObject.SetActive(false);

        if (agent.mesh != null)
            agent.mesh.updateWhenOffscreen = true;

        if (agent.weapons != null)
        {
            agent.weapons.DropWeapon();
            agent.weapons.SetTarget(null);
        }

        if (agent.navMeshAgent != null)
            agent.navMeshAgent.enabled = false;
    }

    public void Update(AiAgent agent) { }

    public void Exit(AiAgent agent)
    {
        if (agent.navMeshAgent != null)
            agent.navMeshAgent.enabled = true;
    }
}