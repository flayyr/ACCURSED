using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BirdController : MonoBehaviour
{
    BirdBaseState currentState;

    public BirdStateIdle IdleState = new BirdStateIdle();
    public BirdStateTakeOff TakeOffState = new BirdStateTakeOff();
    public BirdStateFlying FlyingState = new BirdStateFlying();

    [Header("References")]
    public Animator animator;

    [Header("----------")]

    [Header("Idle Animations")]
    public string[] idleAnimationNames =
    {
        "bird_bob",
        "bird_hop",
        "bird_peck",
        "bird_lookback"
    };

    [Header("Idle Timing")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [HideInInspector] public int lastIdleIndex = -1;
    [HideInInspector] public float idleTimer = 0f;


    private void Awake()
    {
        // mandatory null checking
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentState = IdleState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(BirdBaseState newState)
    {
        // mandatory null checking
        if (currentState != null)
            currentState.ExitState(this);

        currentState = newState;
        newState.EnterState(this);
    }

    public void PlayRandomIdleAnimation()
    {
        // mandatory null checking
        if (animator == null)
        {
            Debug.LogWarning( name + " has no animator.");
            return;
        }

        if (idleAnimationNames == null)
        {
            Debug.LogWarning(name + " has no idle animation.");
            return;
        }

        int nextIdleIndex = Random.Range(0, idleAnimationNames.Length);

        lastIdleIndex = nextIdleIndex;
        
        string animationName = idleAnimationNames[nextIdleIndex];

        animator.Play(animationName);
    }

    public void ResetIdleTimer()
    {
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void OnValidate()
    {
        if (minIdleTime > maxIdleTime)
            minIdleTime = maxIdleTime;
    }
}
