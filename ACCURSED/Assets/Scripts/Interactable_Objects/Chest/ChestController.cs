using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Chest Identity")]
    [Tooltip("Unique ID for this chest. Every chest instance needs a different ID.")]
    [SerializeField] private string chestID;

    [Tooltip("Makes this chest start open before considering save data.")]
    [SerializeField] private bool startsOpen = false;

    [Tooltip("For testing only. Ignores previously saved chest state.")]
    [SerializeField] private bool ignoreSavedState = false;


    [Header("Contained Item")]
    [Tooltip("The actual normal world-item GameObject inside this chest.")]
    [SerializeField] private GameObject containedItem;


    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private string promptText = "[F] Open";
    [SerializeField] private string playerTag = "Player";


    [Header("Animation")]
    [SerializeField] private Animator chestAnimator;

    [SerializeField] private string closedStateName = "ChestClosed";
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private string openStateName = "ChestOpen";

    [Tooltip("Time after opening before the item becomes available.")]
    [Min(0f)]
    [SerializeField] private float itemRevealDelay = 0.35f;


    private readonly HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();

    private Transform currentPlayer;

    private bool isOpen;
    private bool isOpening;
    private bool promptOpen;

    public bool IsOpen => isOpen;


    private void Awake()
    {
        // Automatically find ContainedItem if it was not manually assigned.
        if (containedItem == null)
        {
            Transform itemTransform = transform.Find("ContainedItem");

            if (itemTransform != null)
                containedItem = itemTransform.gameObject;
        }

        if (chestAnimator == null)
            chestAnimator = GetComponent<Animator>();
    }


    private void Start()
    {
        if (string.IsNullOrWhiteSpace(chestID))
            Debug.LogWarning(name + ": Chest ID is empty. Give every chest a unique ID.");

        if (containedItem == null)
            Debug.LogError(name + ": No Contained Item is assigned. " +
                "Put the actual world pickup underneath a child called " + "'ContainedItem' or assign it in the Inspector.");

        bool savedOpen = false;

        if (!ignoreSavedState && !string.IsNullOrWhiteSpace(chestID))
            savedOpen = ChestStateSave.IsChestOpen(chestID);

        isOpen = startsOpen || savedOpen;

        ApplyStartingState();
    }

    private void Update()
    {
        if (currentPlayer == null)
            return;

        if (isOpen || isOpening)
            return;

        if (!promptOpen && !InteractableObjectManager.promptOpen && !GlobalUIController.Instance.CheckIfOtherUIOpen())
            ShowPrompt();

        if (promptOpen && Input.GetKeyDown(interactKey))
            StartCoroutine(OpenChestRoutine());
    }
    
    // Starting State
    private void ApplyStartingState()
    {
        if (isOpen)
        {
            ForceOpenState();

            if (containedItem != null)
                containedItem.SetActive(true);
        }
        else
        {
            ForceClosedState();

            if (containedItem != null)
                containedItem.SetActive(false);
        }
    }
    
    private void ForceClosedState()
    {
        if (chestAnimator == null)
            return;

        if (string.IsNullOrWhiteSpace(closedStateName))
            return;

        int stateHash = Animator.StringToHash(closedStateName);

        if (chestAnimator.HasState(0, stateHash))
        {
            chestAnimator.Play(stateHash, 0, 0f);
            chestAnimator.Update(0f);
        }
        else
        {
            Debug.LogWarning(name + ": Animator does not contain state '" + closedStateName + "'.");
        }
    }

    private void ForceOpenState()
    {
        if (chestAnimator == null)
            return;

        if (string.IsNullOrWhiteSpace(openStateName))
            return;

        int stateHash = Animator.StringToHash(openStateName);

        if (chestAnimator.HasState(0, stateHash))
        {
            // 1f = final frame of opening animation.
            chestAnimator.Play(stateHash, 0, 1f);
            chestAnimator.Update(0f);
        }
        else
        {
            Debug.LogWarning(name + ": Animator does not contain state '" + openStateName + "'.");
        }
    }
    
    // Opening
    private IEnumerator OpenChestRoutine()
    {
        if (isOpen || isOpening)
            yield break;

        isOpening = true;

        HidePrompt();

        if (chestAnimator != null && !string.IsNullOrWhiteSpace(openTriggerName))
            chestAnimator.SetTrigger(openTriggerName);

        // Save the chest as opened immediately.
        isOpen = true;

        if (!string.IsNullOrWhiteSpace(chestID))
            ChestStateSave.MarkChestOpen(chestID);

        // Item remains hidden while lid starts opening.
        if (containedItem != null)
            containedItem.SetActive(false);

        if (itemRevealDelay > 0f)
            yield return new WaitForSeconds(itemRevealDelay);

        RevealContainedItem();

        isOpening = false;
    }

    private void RevealContainedItem()
    {
        if (containedItem == null)
        {
            Debug.LogError( name + ": Tried to reveal contained item, but no item is assigned.");

            return;
        }

        containedItem.SetActive(true);
    }
    
    // Player Detection
    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform player = FindPlayer(other);

        if (player == null)
            return;

        playerColliders.Add(other);
        currentPlayer = player;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Transform player = FindPlayer(other);

        if (player == null)
            return;

        playerColliders.Add(other);
        currentPlayer = player;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!playerColliders.Remove(other))
            return;

        // Player can have multiple colliders.
        if (playerColliders.Count > 0)
            return;

        currentPlayer = null;

        HidePrompt();
    }

    private Transform FindPlayer(Collider2D other)
    {
        if (other == null)
            return null;

        if (other.attachedRigidbody != null)
        {
            Transform player = FindTaggedAncestor(other.attachedRigidbody.transform);

            if (player != null)
                return player;
        }

        return FindTaggedAncestor(other.transform);
    }

    private Transform FindTaggedAncestor(Transform start)
    {
        Transform current = start;

        while (current != null)
        {
            if (current.CompareTag(playerTag))
                return current;

            current = current.parent;
        }

        return null;
    }
    
    // Prompt
    private void ShowPrompt()
    {
        if (promptOpen)
            return;

        if (isOpen || isOpening)
            return;

        if (ToolTipManager.Instance == null)
        {
            Debug.LogWarning(name + ": ToolTipManager was not found.");

            return;
        }

        if (InteractableObjectManager.promptOpen)
            return;

        promptOpen = true;
        InteractableObjectManager.promptOpen = true;

        ToolTipManager.Instance.Prompt(promptText);
    }

    private void HidePrompt()
    {
        if (!promptOpen)
            return;

        promptOpen = false;

        InteractableObjectManager.promptOpen = false;

        if (ToolTipManager.Instance != null)
            ToolTipManager.Instance.ManuallyRemovePrompt();
    }

    private void OnDisable()
    {
        playerColliders.Clear();
        currentPlayer = null;

        HidePrompt();
    }
}