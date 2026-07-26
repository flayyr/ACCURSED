using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomTransitionInteractable : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private string promptText = "[E] Enter";
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Prompt UI - Local To This Script")]
    [SerializeField] private Transform promptParentCanvas;
    [SerializeField] private GameObject promptPrefab;

    [Header("Scene Transition")]
    [SerializeField] private string targetSceneName = "AltarInterior";
    [SerializeField] private string targetSpawnID = "Entrance";

    [Tooltip("Enable this when the same player object should be carried into the destination scene.")]
    [SerializeField] private bool moveCurrentPlayerToNextScene = true;

    [Header("Gate Animation")]
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private string gateOpenTriggerName = "Open";

    [Tooltip("Optional placeholder visual enabled when the interaction begins.")]
    [SerializeField] private GameObject gateOpenPlaceholderIndicator;

    [Min(0f)][SerializeField] private float gateOpenDelay = 0.35f;

    private readonly HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();

    private bool isTransitioning;
    private Transform currentPlayer;
    private GameObject currentPrompt;

    private void Reset()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;
    }

    private void Update()
    {
        if (currentPlayer == null || isTransitioning)
            return;

        if (RoomTransitionManager.Instance != null && RoomTransitionManager.Instance.IsTransitioning)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
            StartCoroutine(EnterRoomRoutine(currentPlayer));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
            return;

        Transform player = FindTaggedPlayerTransform(other);

        if (player == null)
            return;

        playerColliders.Add(other);
        currentPlayer = player;
        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!playerColliders.Remove(other))
            return;

        if (playerColliders.Count > 0)
            return;

        currentPlayer = null;
        HidePrompt();
    }

    private void OnDisable()
    {
        playerColliders.Clear();
        currentPlayer = null;
        HidePrompt();
    }

    private IEnumerator EnterRoomRoutine(Transform playerAtInteractionTime)
    {
        isTransitioning = true;
        HidePrompt();
        PlayGateOpenFeedback();

        if (gateOpenDelay > 0f)
            yield return new WaitForSecondsRealtime(gateOpenDelay);

        RoomTransitionManager manager = RoomTransitionManager.Instance;

        if (manager == null)
        {
            Debug.LogError(name + ": No RoomTransitionManager exists. Add its prefab to the scene.");
            isTransitioning = false;
            yield break;
        }

        bool transitionStarted;

        if (moveCurrentPlayerToNextScene)
        {
            if (playerAtInteractionTime == null)
            {
                Debug.LogError($"{name}: The player reference was lost before the transition began.");
                isTransitioning = false;
                yield break;
            }

            transitionStarted = manager.BeginTransition(targetSceneName, targetSpawnID, playerAtInteractionTime);
        }
        else
        {
            transitionStarted = manager.BeginTransition(targetSceneName);
        }

        if (!transitionStarted)
            isTransitioning = false;
    }

    private Transform FindTaggedPlayerTransform(Collider2D other)
    {
        if (other == null)
            return null;

        // Prefer the Rigidbody2D object, which is usually the real Player root.
        if (other.attachedRigidbody != null)
        {
            Transform rigidbodyTransform = FindTaggedAncestor(other.attachedRigidbody.transform);

            if (rigidbodyTransform != null)
                return rigidbodyTransform;
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

    private void ShowPrompt()
    {
        if (currentPrompt != null)
            return;

        if (promptParentCanvas == null)
        {
            Debug.LogError($"{name}: promptParentCanvas is not assigned.");
            return;
        }

        if (promptPrefab == null)
        {
            Debug.LogError($"{name}: promptPrefab is not assigned.");
            return;
        }

        currentPrompt = Instantiate(promptPrefab, promptParentCanvas, false);
        currentPrompt.SetActive(true);

        PromptUI promptUI = currentPrompt.GetComponentInChildren<PromptUI>(true);

        if (promptUI != null)
            promptUI.SetText(promptText);
        else
            Debug.LogWarning($"{name}: promptPrefab does not contain a PromptUI component.");
    }

    private void HidePrompt()
    {
        if (currentPrompt == null)
            return;

        Destroy(currentPrompt);
        currentPrompt = null;
    }

    private void PlayGateOpenFeedback()
    {
        if (gateAnimator != null && !string.IsNullOrWhiteSpace(gateOpenTriggerName))
            gateAnimator.SetTrigger(gateOpenTriggerName);

        if (gateOpenPlaceholderIndicator != null)
            gateOpenPlaceholderIndicator.SetActive(true);
    }
}
