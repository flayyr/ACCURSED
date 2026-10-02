using UnityEngine;

public class AbilityTriggeredSwitch : MonoBehaviour
{
    public enum TriggerCondition
    {
        VestigeHit,
        RemembranceHit,
        PlayerHealNearby
    }

    [Header("Starting State")]
    [SerializeField] private bool startActivated = false;

    [Header("Trigger Condition")]
    [SerializeField] private TriggerCondition triggerCondition;

    [Tooltip("If enabled, triggering while active will deactivate it.")]
    [SerializeField] private bool toggleOnTrigger = true;

    [Header("Visual / Animator")]
    [Tooltip("Child object containing the visual and Animator.")]
    [SerializeField] private GameObject animatedChild;

    [SerializeField] private Animator animator;

    [Header("Animator State Names")]
    [SerializeField] private string inactiveIdleAnimation = "InactiveIdle";
    [SerializeField] private string activationAnimation = "Activate";
    [SerializeField] private string activeIdleAnimation = "ActiveIdle";
    [SerializeField] private string deactivationAnimation = "InactiveIdle";

    [Header("Heal Trigger")]
    [SerializeField] private float healDetectionRadius = 4f;

    [Header("Runtime Inspector Testing")]
    [Tooltip("Enable this to allow the Inspector test switches below during Play Mode.")]
    [SerializeField] private bool allowRuntimeInspectorControls = true;

    [Tooltip("Turn this on during Play Mode to activate the object.")]
    [SerializeField] private bool inspectorActivate;

    [Tooltip("Turn this on during Play Mode to deactivate the object.")]
    [SerializeField] private bool inspectorDeactivate;

    [Tooltip("Turn this on during Play Mode to toggle the object's current state.")]
    [SerializeField] private bool inspectorToggle;

    [Header("Runtime State - Read Only")]
    [SerializeField] private bool isActivated;

    private PlayerAbilities playerAbilities;
    private Transform playerTransform;

    public bool IsActivated => isActivated;

    private void Awake()
    {
        if (animator == null && animatedChild != null)
            animator = animatedChild.GetComponent<Animator>();

        SetStartingState();
    }

    private void Start()
    {
        SetupHealListener();
    }

    private void Update()
    {
        HandleRuntimeInspectorControls();
    }

    private void OnDestroy()
    {
        if (playerAbilities != null)
            playerAbilities.OnHealUsed -= OnPlayerHeal;
    }

    // =========================================================
    // RUNTIME INSPECTOR CONTROLS
    // =========================================================

    private void HandleRuntimeInspectorControls()
    {
        if (!allowRuntimeInspectorControls)
            return;

        // Activate switch
        if (inspectorActivate)
        {
            inspectorActivate = false;
            Activate();
        }

        // Deactivate switch
        if (inspectorDeactivate)
        {
            inspectorDeactivate = false;
            Deactivate();
        }

        // Toggle switch
        if (inspectorToggle)
        {
            inspectorToggle = false;
            Toggle();
        }
    }

    // =========================================================
    // STARTING STATE
    // =========================================================

    private void SetStartingState()
    {
        isActivated = startActivated;

        if (animator == null)
            return;

        if (isActivated)
        {
            if (!string.IsNullOrEmpty(activeIdleAnimation))
                animator.Play(activeIdleAnimation, 0, 0f);
        }
        else
        {
            if (!string.IsNullOrEmpty(inactiveIdleAnimation))
                animator.Play(inactiveIdleAnimation, 0, 0f);
        }
    }

    // =========================================================
    // VESTIGE
    // =========================================================

    public void VestigeHit()
    {
        if (triggerCondition != TriggerCondition.VestigeHit)
            return;

        TriggerSwitch();
    }

    // =========================================================
    // REMEMBRANCE
    // =========================================================

    public void RemembranceHit()
    {
        if (triggerCondition != TriggerCondition.RemembranceHit)
            return;

        TriggerSwitch();
    }

    // =========================================================
    // HEAL
    // =========================================================

    private void SetupHealListener()
    {
        if (triggerCondition != TriggerCondition.PlayerHealNearby)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning($"{name}: Couldn't find Player.");
            return;
        }

        playerTransform = player.transform;
        playerAbilities = player.GetComponent<PlayerAbilities>();

        if (playerAbilities == null)
        {
            Debug.LogWarning($"{name}: Player doesn't have PlayerAbilities.");
            return;
        }

        playerAbilities.OnHealUsed += OnPlayerHeal;
    }

    private void OnPlayerHeal()
    {
        if (playerTransform == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            playerTransform.position
        );

        if (distance <= healDetectionRadius)
            PlayerHealedNearby();
    }

    public void PlayerHealedNearby()
    {
        if (triggerCondition != TriggerCondition.PlayerHealNearby)
            return;

        TriggerSwitch();
    }

    // =========================================================
    // SWITCH LOGIC
    // =========================================================

    private void TriggerSwitch()
    {
        if (toggleOnTrigger)
        {
            Toggle();
        }
        else
        {
            Activate();
        }
    }

    public void Activate()
    {
        SetActivated(true);
    }

    public void Deactivate()
    {
        SetActivated(false);
    }

    public void Toggle()
    {
        SetActivated(!isActivated);
    }

    public void SetActivated(bool activated)
    {
        // Don't replay the animation if we're already in that state.
        if (isActivated == activated)
            return;

        isActivated = activated;

        if (isActivated)
            PlayActivation();
        else
            PlayDeactivation();
    }

    // =========================================================
    // ANIMATION
    // =========================================================

    private void PlayActivation()
    {
        if (animator == null)
            return;

        if (!string.IsNullOrEmpty(activationAnimation))
            animator.Play(activationAnimation, 0, 0f);
    }

    private void PlayDeactivation()
    {
        if (animator == null)
            return;

        if (!string.IsNullOrEmpty(deactivationAnimation))
            animator.Play(deactivationAnimation, 0, 0f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (triggerCondition == TriggerCondition.PlayerHealNearby)
            Gizmos.DrawWireSphere(transform.position, healDetectionRadius);
    }
#endif
}