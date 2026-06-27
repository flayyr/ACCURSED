using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomTransitionInteractable : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private string promptText = "[E] Enter";
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Scene Transition")]
    [SerializeField] private string targetSceneName = "AltarInterior";
    [SerializeField] private string targetSpawnID = "Entrance";

    [Tooltip("Use true if the player should be carried into the next scene. Useful if AltarInterior is empty.")]
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

        ToolTipManager.Instance.Prompt(promptText);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        playerInRange = false;
        currentPlayer = null;

        ToolTipManager.Instance.ManuallyRemovePrompt();
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

    private IEnumerator EnterRoomRoutine()
    {
        isTransitioning = true;

        ToolTipManager.Instance.ManuallyRemovePrompt();

        PlayGateOpenPlaceholder();

        yield return new WaitForSeconds(gateOpenDelay);

        if (RoomTransitionManager.Instance == null)
        {
            Debug.LogError("No RoomTransitionManager found in the scene.");
            yield break;
        }

        Transform playerToMove = moveCurrentPlayerToNextScene ? currentPlayer : null;

        yield return RoomTransitionManager.Instance.TransitionToScene(
            targetSceneName,
            targetSpawnID,
            playerToMove
        );
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