using System;
using UnityEngine;

public enum StatueVisionState
{
    Blind,
    BlindBroken,
    Seeing
}

public class StatueStateController : MonoBehaviour
{
    [Header("State")]
    [Tooltip("Can be changed in the Inspector. During Play Mode, changing this value applies the new state immediately.")]
    [SerializeField] private StatueVisionState currentState = StatueVisionState.Blind;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite blindSprite;
    [SerializeField] private Sprite blindBrokenSprite;
    [SerializeField] private Sprite seeingSprite;

    [Header("Interaction")]
    [Min(0.1f)]
    [SerializeField] private float interactionDistance = 2f;

    [Min(0f)]
    [Tooltip("How long the player must wait after a successful state change before interacting with this statue again.")]
    [SerializeField] private float stateChangeCooldown = 3f;

    [SerializeField] private string unveilPrompt = "Unveil";
    [SerializeField] private string shroudPrompt = "Shroud";
    [SerializeField] private string repairPrompt = "Repair";

    [Header("Repair Material")]
    [Tooltip("Assign the exact ItemPickupSO asset required to repair this statue. The controller reads its itemID automatically.")]
    [SerializeField] private ItemPickupSO repairItem;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private StatueSeeingField seeingField;
    [SerializeField] private StatueInteractionMessageUI feedbackUI;

    private StatueVisionState appliedState;
    private bool hasAppliedState;
    private bool ownsPrompt;
    private float nextInteractionTime;

    private static StatueStateController activeStatuePrompt;

    public StatueVisionState CurrentState => currentState;
    public bool IsSeeing => currentState == StatueVisionState.Seeing;
    public bool IsBroken => currentState == StatueVisionState.BlindBroken;
    public bool IsOnCooldown => Time.time < nextInteractionTime;

    public event Action<StatueVisionState> StateChanged;

    private void Awake()
    {
        ResolveReferences();
        ApplyState(currentState, true);
    }

    private void Update()
    {
        // This also makes changing Current State in the Inspector during Play Mode work immediately.
        if (!hasAppliedState || currentState != appliedState)
        {
            ClosePrompt();
            ApplyState(currentState, false);
        }

        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        if (playerInventory == null)
            playerInventory = player.GetComponent<PlayerInventory>();

        float distanceSqr = (player.position - transform.position).sqrMagnitude;
        float interactionDistanceSqr = interactionDistance * interactionDistance;

        bool closeEnough = distanceSqr <= interactionDistanceSqr;
        bool feedbackBlocking = feedbackUI != null && feedbackUI.IsShowing;

        if (closeEnough && !IsOnCooldown && !feedbackBlocking)
            TryOpenPrompt();
        else
            ClosePrompt();
    }

    private void ResolveReferences()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (seeingField == null)
            seeingField = GetComponentInChildren<StatueSeeingField>(true);

        if (feedbackUI == null)
            feedbackUI = StatueInteractionMessageUI.Instance;

        if (player == null)
            FindPlayer();

        if (playerInventory == null && player != null)
            playerInventory = player.GetComponent<PlayerInventory>();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject == null)
            return;

        player = playerObject.transform;

        if (playerInventory == null)
            playerInventory = playerObject.GetComponent<PlayerInventory>();
    }

    private void TryOpenPrompt()
    {
        if (ownsPrompt)
            return;

        if (ToolTipManager.Instance == null)
            return;

        if (activeStatuePrompt != null && activeStatuePrompt != this)
            return;

        if (ToolTipManager.Instance.IsPromptOpen)
            return;

        if (GlobalUIController.Instance != null && GlobalUIController.Instance.CheckIfOtherUIOpen())
            return;

        string promptText = GetPromptText();

        if (string.IsNullOrWhiteSpace(promptText))
            return;

        activeStatuePrompt = this;
        ownsPrompt = true;

        // Your existing ToolTipManager handles the interact key and adds the [F] presentation.
        ToolTipManager.Instance.Prompt(promptText, HandleInteraction);
    }

    private string GetPromptText()
    {
        switch (currentState)
        {
            case StatueVisionState.Blind:
                return unveilPrompt;

            case StatueVisionState.BlindBroken:
                return repairPrompt;

            case StatueVisionState.Seeing:
                return shroudPrompt;

            default:
                return string.Empty;
        }
    }

    private void HandleInteraction()
    {
        // Remove the actionable prompt before changing state or showing a message.
        ClosePrompt();

        if (IsOnCooldown)
            return;

        switch (currentState)
        {
            case StatueVisionState.Blind:
                ChangeStateFromGameplay(StatueVisionState.Seeing);
                break;

            case StatueVisionState.Seeing:
                ChangeStateFromGameplay(StatueVisionState.Blind);
                break;

            case StatueVisionState.BlindBroken:
                TryRepair();
                break;
        }
    }

    private void TryRepair()
    {
        if (repairItem == null)
        {
            Debug.LogError($"{name} has no repair ItemPickupSO assigned.", this);
            ShowFeedback("Insufficient repair materials");
            return;
        }

        if (string.IsNullOrWhiteSpace(repairItem.itemID))
        {
            Debug.LogError($"Repair item '{repairItem.name}' has no itemID.", repairItem);
            ShowFeedback("Insufficient repair materials");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogError($"{name} could not find PlayerInventory.", this);
            ShowFeedback("Insufficient repair materials");
            return;
        }

        // The statue references the ScriptableObject asset; its ID is never hand-typed here.
        if (!playerInventory.HasItem(repairItem.itemID))
        {
            ShowFeedback("Insufficient repair materials");
            return;
        }

        if (!playerInventory.RemoveItem(repairItem.itemID, 1))
        {
            ShowFeedback("Insufficient repair materials");
            return;
        }

        string displayName = string.IsNullOrWhiteSpace(repairItem.itemName)
            ? repairItem.name
            : repairItem.itemName;

        ChangeStateFromGameplay(StatueVisionState.Blind);
        ShowFeedback($"Used {displayName} to repair");
    }

    private void ChangeStateFromGameplay(StatueVisionState newState)
    {
        currentState = newState;
        ApplyState(newState, false);
        nextInteractionTime = Time.time + stateChangeCooldown;
    }

    /// <summary>
    /// Lets another gameplay script force a state without waiting for the interaction cooldown.
    /// Set animateField=false if loading/restoring saved state and you want the visual to appear immediately.
    /// </summary>
    public void ForceState(StatueVisionState newState, bool animateField = true)
    {
        ClosePrompt();
        currentState = newState;
        ApplyState(newState, !animateField);
    }

    private void ApplyState(StatueVisionState state, bool instantField)
    {
        appliedState = state;
        hasAppliedState = true;

        ApplySprite(state);

        if (seeingField != null)
            seeingField.SetSeeing(state == StatueVisionState.Seeing, instantField);

        StateChanged?.Invoke(state);
    }

    private void ApplySprite(StatueVisionState state)
    {
        if (spriteRenderer == null)
            return;

        switch (state)
        {
            case StatueVisionState.Blind:
                spriteRenderer.sprite = blindSprite;
                break;

            case StatueVisionState.BlindBroken:
                spriteRenderer.sprite = blindBrokenSprite;
                break;

            case StatueVisionState.Seeing:
                spriteRenderer.sprite = seeingSprite;
                break;
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackUI == null)
            feedbackUI = StatueInteractionMessageUI.Instance;

        if (feedbackUI != null)
        {
            feedbackUI.ShowMessage(message);
        }
        else
        {
            Debug.Log(message, this);
        }
    }

    private void ClosePrompt()
    {
        if (!ownsPrompt)
            return;

        if (ToolTipManager.Instance != null)
            ToolTipManager.Instance.ManuallyRemovePrompt();

        ownsPrompt = false;

        if (activeStatuePrompt == this)
            activeStatuePrompt = null;
    }

    private void OnDisable()
    {
        ClosePrompt();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Edit-mode preview of the selected state.
        if (!Application.isPlaying)
            ApplySprite(currentState);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
#endif
}
