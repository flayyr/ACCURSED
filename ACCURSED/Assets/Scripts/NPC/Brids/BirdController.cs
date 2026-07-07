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

    [HideInInspector] public bool playingAnimation = false;
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

        // randomly decide face direction (left, right)
        int faceDir = Random.Range(0,2);

        if (faceDir == 1)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
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

    public void PlayRandomAnimation()
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

        playingAnimation = true;

        // choose a random animation to play
        int nextIdleIndex = Random.Range(0, idleAnimationNames.Length);

        lastIdleIndex = nextIdleIndex;
        
        string animationName = idleAnimationNames[nextIdleIndex];

        animator.Play(animationName, 0, 0f);

        Debug.Log("playing " + animationName);
    }

    public void ResetIdleTimer()
    {
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    public void AnimationEnd()
    {
        Debug.Log("AnimationEnd called on " + name);

        ResetIdleTimer();

        playingAnimation = false;
    }

    private void OnValidate()
    {
        if (minIdleTime > maxIdleTime)
            minIdleTime = maxIdleTime;
    }
}
