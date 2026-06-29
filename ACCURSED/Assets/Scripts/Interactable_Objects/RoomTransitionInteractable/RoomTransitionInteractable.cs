using System.Collections;
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

    [Tooltip("Use true if the player should be carried into the next scene.")]
    [SerializeField] private bool moveCurrentPlayerToNextScene = true;

    [Header("Gate Animation Placeholder")]
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private string gateOpenTriggerName = "Open";

    [Tooltip("Optional placeholder object. Example: a glowing sprite, text, or simple visual that turns on when the gate opens.")]
    [SerializeField] private GameObject gateOpenPlaceholderIndicator;

    [SerializeField] private float gateOpenDelay = 0.35f;

    private bool playerInRange;
    private bool isTransitioning;
    private Transform currentPlayer;

    private GameObject currentPrompt;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;
        if (isTransitioning) return;

        playerInRange = true;
        currentPlayer = collision.transform;

        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        playerInRange = false;
        currentPlayer = null;

        HidePrompt();
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (isTransitioning) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartCoroutine(EnterRoomRoutine());
        }
    }

    private void ShowPrompt()
    {
        if (currentPrompt != null) return;

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
        {
            promptUI.SetText(promptText);
        }
        else
        {
            Debug.LogWarning($"{name}: promptPrefab does not have a PromptUI component.");
        }
    }

    private void HidePrompt()
    {
        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
            currentPrompt = null;
        }
    }

    private IEnumerator EnterRoomRoutine()
    {
        isTransitioning = true;

        HidePrompt();

        PlayGateOpenPlaceholder();

        yield return new WaitForSeconds(gateOpenDelay);

        if (RoomTransitionManager.Instance == null)
        {
            Debug.LogError("No RoomTransitionManager found in the scene.");
            yield break;
        }

        Transform playerToMove = moveCurrentPlayerToNextScene ? currentPlayer : null;

        RoomTransitionManager.Instance.BeginTransition(targetSceneName, targetSpawnID, playerToMove);
    }

    private void PlayGateOpenPlaceholder()
    {
        if (gateAnimator != null)
        {
            gateAnimator.SetTrigger(gateOpenTriggerName);
        }
        else
        {
            Debug.Log("Gate opening placeholder: gate would open now.");
        }

        if (gateOpenPlaceholderIndicator != null)
        {
            gateOpenPlaceholderIndicator.SetActive(true);
        }
    }
}