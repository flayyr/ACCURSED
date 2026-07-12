using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BirdController : MonoBehaviour
{
    BirdBaseState currentState;

    public BirdStateIdle IdleState = new BirdStateIdle();
    public BirdStateTakeOff TakeOffState = new BirdStateTakeOff();
    public BirdStateFlying FlyingState = new BirdStateFlying();

    [Header("Flying stats")]
    public float flySpeed = 5f;
    public float targetFlySpeed;

    public float upwardForce = 0.35f;
    public float targetUpwardForce;

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
    public float minFlySpeed = 5.5f;
    public float maxFlySpeed = 8f;
    
    public float minUpwardForce = -0.75f;
    public float maxUpwardForce = 0.75f;

    public float detectionDistance = 7.5f;

    [Header("Flying Fluctuation")]
    public bool useFlightFluctuation = true;

    public float minFluctuationTime = 0.5f;
    public float maxFluctuationTime = 1.5f;

    public float speedChangeRate = 5f;
    public float upwardForceChangeRate = 1f;

    [Header("Flying Fade")]

    [Tooltip("How long the bird flies before it begins fading.")]
    [Min(0f)] public float flyFadeDelay = 2f;

    [Tooltip("How long the bird takes to fade completely.")]
    [Min(0.01f)] public float flyFadeDuration = 1.5f;

    [Tooltip("All SpriteRenderers that should fade")]
    [SerializeField] private SpriteRenderer[] birdSpriteRenderers;

    private float[] originalRendererAlphas;


    [HideInInspector] public bool playingAnimation = false;
    [HideInInspector] public int lastIdleIndex = -1;
    [HideInInspector] public float idleTimer = 0f;

    [HideInInspector] public bool playerDetected = false;

    [HideInInspector] public Vector2 flyDirection = Vector2.right;
    [HideInInspector] public Vector2 direction;
    
    [HideInInspector] public float flightFluctuationTimer = 0f;

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
        direction.y += upwardForce;

        // randomize speed and upward force a little bit
        RandomizeFlightValues();
    }

    public void RandomizeFlightValues()
    {
        flySpeed = Random.Range(minFlySpeed, maxFlySpeed);
        upwardForce = Random.Range(minUpwardForce, maxUpwardForce);

        targetFlySpeed = flySpeed;
        targetUpwardForce = upwardForce;

        flightFluctuationTimer = Random.Range(minFluctuationTime, maxFluctuationTime);
    }

    public void UpdateFlightFluctuation()
    {
        if (!useFlightFluctuation)
            return;

        flightFluctuationTimer -= Time.deltaTime;

        if (flightFluctuationTimer <= 0f)
        {
            targetFlySpeed = Random.Range(minFlySpeed, maxFlySpeed);
            targetUpwardForce = Random.Range(minUpwardForce, maxUpwardForce);
            
            flightFluctuationTimer = Random.Range(minFluctuationTime, maxFluctuationTime);
        }

        flySpeed = Mathf.MoveTowards(flySpeed, targetFlySpeed, speedChangeRate * Time.deltaTime);

        upwardForce = Mathf.MoveTowards(upwardForce, targetUpwardForce, upwardForceChangeRate * Time.deltaTime);
    }

    public Vector2 GetCurrentFlyDirection()
    {
        Vector2 currentDir = direction;

        currentDir.y += upwardForce;

        return currentDir.normalized;
    }

    public void InitializeFade()
    {
        if (birdSpriteRenderers == null || birdSpriteRenderers.Length == 0)
            birdSpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        originalRendererAlphas = new float[birdSpriteRenderers.Length];

        for (int i = 0; i < birdSpriteRenderers.Length; i++)
        {
            if (birdSpriteRenderers[i] == null)
                continue;

            originalRendererAlphas[i] = birdSpriteRenderers[i].color.a;
        }

        SetFlightFade(1f);
    }

    public void SetFlightFade(float FadeAmount)
    {
        FadeAmount = Mathf.Clamp01(FadeAmount);

        if (birdSpriteRenderers == null)
            return;

        for (int i = 0; i < birdSpriteRenderers.Length; i++)
        {
            SpriteRenderer spriteRenderer = birdSpriteRenderers[i];

            if (spriteRenderer == null) 
                continue;

            Color color = spriteRenderer.color;

            float originalAlpha = 1f;

            if (originalRendererAlphas != null && i < originalRendererAlphas.Length)
                originalAlpha = originalRendererAlphas[i];

            color.a = originalAlpha * FadeAmount;

            spriteRenderer.color = color;
        }
    }

    private void OnValidate()
    {
        if (minIdleTime > maxIdleTime)
            minIdleTime = maxIdleTime;

        if (minFlySpeed > maxFlySpeed)
            minFlySpeed = maxFlySpeed;

        if (minUpwardForce > maxUpwardForce)
            minUpwardForce = maxUpwardForce;

        if (minFluctuationTime > maxFluctuationTime)
            minFluctuationTime = maxFluctuationTime;
    }           
}
