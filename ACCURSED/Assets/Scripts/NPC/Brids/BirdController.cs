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
    public Transform birdTransform;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private string playerTag = "Player";

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

    [Header("----------")]

    [Header("Take Off Animations")]
    public string takeOffAnimationName = "bird_takeoff";

    [Header("----------")]

    [Header("Flying Animations")]
    public string flyAnimationName = "bird_fly";

    [Header("Flying Settings")]
    public float flySpeed = 5f;
    public float minFlySpeed = 3.5f;
    public float maxFlySpeed = 7f;
    public float upwardFlyBias = 0.35f;
    public float detectionDistance = 7.5f;

    [HideInInspector] public bool playingAnimation = false;
    [HideInInspector] public int lastIdleIndex = -1;
    [HideInInspector] public float idleTimer = 0f;
    [HideInInspector] public bool playerDetected = false;
    [HideInInspector] public Vector2 flyDirection = Vector2.right;
    [HideInInspector] public Vector2 direction;

    private void Awake()
    {
        // mandatory null checking 
        if (animator == null)
            animator = GetComponent<Animator>();

        if (birdTransform == null)
            birdTransform = transform;

        // find the player
        if (playerTransform == null)
        {
            FindPlayer();
        }
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

        // randomly select a flying speed
        flySpeed = Random.Range(minFlySpeed, maxFlySpeed);
    }

    private void Update()
    {
        UpdatePlayerDetection();

        currentState.UpdateState(this);
    }

    private void UpdatePlayerDetection()
    {
        if (playerTransform == null)
        {
            FindPlayer();

            playerDetected = false;
            return;
        }

        float distance = Vector2.Distance(playerTransform.position, birdTransform.position);
        playerDetected = distance <= detectionDistance;
    }

    public void SwitchState(BirdBaseState newState)
    {
        // mandatory null checking
        if (currentState != null)
            currentState.ExitState(this);

        currentState = newState;
        currentState.EnterState(this);
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

        if (currentState != null)
        {
            currentState.AnimationEnd(this);
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    public void SetFlyDirection()
    {
        

        if (playerTransform != null)
        {
            direction = (Vector2)(birdTransform.position - playerTransform.position);
        }
        else
        {
            direction = Random.value > 0.5f ? Vector2.right : Vector2.left;
        }

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = Random.value > 0.5f ? Vector2.right : Vector2.left;
        }

        // Adds a slight upward force so the bird does not fly perfectly flat
        direction.y += upwardFlyBias;
    }

    private void OnValidate()
    {
        if (minIdleTime > maxIdleTime)
            minIdleTime = maxIdleTime;
    }
}
