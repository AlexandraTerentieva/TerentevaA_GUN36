using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public AiStateId initialState;
    public AiAgentConfig config;
    public bool isMelee = false;

    [HideInInspector] public AiStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Ragdoll ragdoll;
    [HideInInspector] public SkinnedMeshRenderer mesh;
    [HideInInspector] public UIHealthBar ui;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public AiWeapons weapons;
    [HideInInspector] public AiSensor sensor;
    [HideInInspector] public AiTargetingSystem targeting;
    [HideInInspector] public AiHealth health;

    void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        ui = GetComponentInChildren<UIHealthBar>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        weapons = GetComponent<AiWeapons>();
        sensor = GetComponent<AiSensor>();
        targeting = GetComponent<AiTargetingSystem>();
        health = GetComponent<AiHealth>();

        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiIdleState());

        // 👇 ТОЛЬКО НЕ БЛИЖНИКИ ИЩУТ ОРУЖИЕ
        if (!isMelee)
        {
            stateMachine.RegisterState(new AiFindWeaponState());
        }

        stateMachine.RegisterState(new AiFindTargetState());
        stateMachine.RegisterState(new AiChasePlayerState());

        if (isMelee)
        {
            stateMachine.RegisterState(new AiMeleeAttackState());
        }
        else
        {
            stateMachine.RegisterState(new AiAttackTargetState());
        }

        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiFindHealthState());
        stateMachine.RegisterState(new AiFindAmmoState());

        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();
    }
}