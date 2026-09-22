using System;
using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public enum OnInteractType
    {
        Dialog,
        AnimationTransition
    }

    [Serializable]
    public class NPCSpawnState
    {
        [AnimatorState]
        public string animationName;
        public Vector2 position;
        public float chanceOfState = 1f;

        public string promptAction;
        public OnInteractType onInteract;

        [FieldShowIf(nameof(onInteract), OnInteractType.AnimationTransition)]
        [AnimatorParam(AnimatorControllerParameterType.Trigger)]
        public string animationTrigger;
    }

    [Serializable]
    public class AutoStateTransition
    {
        [AnimatorState]
        public string fromState;

        [AnimatorParam(AnimatorControllerParameterType.Trigger)]
        public string toState;

        public Vector2 transitionTimeRange = new Vector2(1f, 3f);

        [HideInInspector] public float timer;

        private float targetTime;

        public float GetTargetTime() => targetTime;

        public void RollTargetTime()
        {
            targetTime = UnityEngine.Random.Range(transitionTimeRange.x, transitionTimeRange.y);
        }

        public void ResetTimer()
        {
            timer = 0f;
            RollTargetTime();
        }
    }

    [SerializeField] protected List<NPCSpawnState> spawnStates = new List<NPCSpawnState>();
    [SerializeField] protected List<AutoStateTransition> autoStateTransitions = new List<AutoStateTransition>();
    [SerializeField] protected Animator animator;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private string playerTag = "Player";

    private readonly HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();
    private Transform currentPlayer;
    bool promptOpen;

    [SerializeField] protected NPCSpawnState currentState;

    [Button("Reroll Spawn")]
    private void RerollStart()
    {
        Start();
    }

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        currentState = ChooseSpawnState();
        if (currentState == null) return;

        transform.position = currentState.position;

        if (animator != null && !string.IsNullOrEmpty(currentState.animationName))
            animator.Play(currentState.animationName);

        if (autoStateTransitions != null)
        {
            foreach (AutoStateTransition transition in autoStateTransitions)
                transition.RollTargetTime();
        }
    }

    void Update()
    {
        UpdateCurrentState();
        AutoTransitionStateUpdate();
        InteractionUpdate();
    }

    private void InteractionUpdate()
    {
        if (currentPlayer == null || currentState == null)
        {
            HidePrompt();
            return;
        }

        if (!promptOpen && !InteractableObjectManager.promptOpen && !GlobalUIController.Instance.CheckIfOtherUIOpen())
            ShowPrompt();

        if (promptOpen && Input.GetKeyDown(interactKey))
            Interact();
    }

    private void Interact()
    {
        switch (currentState.onInteract)
        {
            case OnInteractType.AnimationTransition:
                if (animator != null && !string.IsNullOrEmpty(currentState.animationTrigger))
                    animator.SetTrigger(currentState.animationTrigger);
                break;

            case OnInteractType.Dialog:
                // IMPLEMENT DIALOG SYSTEM START
                Debug.Log("happening");
                GameObject.Find("NPC Text Controller").GetComponent<DialogueSpawner>().checkInput();
                break;
        }
    }

    private void UpdateCurrentState()
    {
        if (animator == null || spawnStates == null) return;

        currentState = spawnStates.Find(state =>
            !string.IsNullOrEmpty(state.animationName) &&
            animator.GetCurrentAnimatorStateInfo(0).IsName(state.animationName));
    }

    void AutoTransitionStateUpdate()
    {
        if (animator == null || autoStateTransitions == null) return;

        foreach (AutoStateTransition transition in autoStateTransitions)
        {
            if (string.IsNullOrEmpty(transition.fromState)) continue;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName(transition.fromState))
            {
                transition.timer += Time.deltaTime;

                if (transition.timer >= transition.GetTargetTime())
                {
                    if (!string.IsNullOrEmpty(transition.toState))
                        animator.SetTrigger(transition.toState);

                    transition.ResetTimer();
                }
            }
            else
            {
                transition.ResetTimer();
            }
        }
    }

    private NPCSpawnState ChooseSpawnState()
    {
        if (spawnStates == null || spawnStates.Count == 0) return null;

        float totalWeight = 0f;
        foreach (NPCSpawnState state in spawnStates)
            totalWeight += Mathf.Max(0f, state.chanceOfState);

        if (totalWeight <= 0f) return spawnStates[0];

        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (NPCSpawnState state in spawnStates)
        {
            cumulative += Mathf.Max(0f, state.chanceOfState);
            if (roll <= cumulative)
                return state;
        }

        return spawnStates[spawnStates.Count - 1];
    }

    private void ShowPrompt()
    {
        if (promptOpen)
            return;

        if (currentState == null)
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

        ToolTipManager.Instance.Prompt(currentState.promptAction);
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

    private void OnDisable()
    {
        playerColliders.Clear();
        currentPlayer = null;

        HidePrompt();
    }
}
