using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class AiAgent : MonoBehaviour
{
    [Header("Настройки")]
    public float idleDuration = 3f;
    public float collectRadius = 5f;
    public int maxCollectibles = 5;
    public Transform returnPoint;
    public TMP_Text scoreText;

    [Header("Компоненты")]
    public NavMeshAgent agent;
    public Animator animator;

    [HideInInspector] public AiStateMachine stateMachine;
    [HideInInspector] public GameObject targetCollectible;
    [HideInInspector] public int collectedCount = 0;

    // Хеши анимаций
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int SearchHash = Animator.StringToHash("Search");
    private static readonly int CollectHash = Animator.StringToHash("Collect");
    private static readonly int ReturnHash = Animator.StringToHash("Return");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiSearchState());
        stateMachine.RegisterState(new AiCollectState());
        stateMachine.RegisterState(new AiReturnState());

        stateMachine.ChangeState(AiStateId.Idle);
        UpdateScoreUI();
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Собрано: {collectedCount}/{maxCollectibles}";
    }

    public void SetAnimationTrigger(string trigger)
    {
        if (animator != null)
        {
            int hash = Animator.StringToHash(trigger);
            animator.SetTrigger(hash);
        }
    }
}