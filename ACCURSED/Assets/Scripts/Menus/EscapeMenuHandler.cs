using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EscapeMenuHandler : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private string startScreenName = "StartMenu";

    [Header("Input")]
    [Tooltip("Disable this when a UI Button calls ReturnToStartMenu instead.")]
    [SerializeField] private bool listenForEscapeKey = true;
    [SerializeField] private KeyCode escapeKey = KeyCode.Escape;

    [Header("Paused Menus")]
    [Tooltip("Restores normal time before leaving a pause menu.")]
    [SerializeField] private bool restoreTimeScale = true;

    private bool transitionRequested;

    private void Update()
    {
        if (!listenForEscapeKey || transitionRequested || SaveNamePopup.BlocksMenuInput)
            return;

        if (Input.GetKeyDown(escapeKey) ||  (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            ReturnToStartMenu();
    }

    public void ReturnToStartMenu()
    {
        if (transitionRequested)
            return;

        if (SceneManager.GetActiveScene().name == startScreenName)
            return;

        RoomTransitionManager manager = RoomTransitionManager.Instance;

        if (manager == null)
        {
            Debug.LogError("EscapeMenuHandler: No RoomTransitionManager exists. " +
                "Add the RoomTransitionManager prefab to the scene.");
            return;
        }

        if (manager.IsTransitioning)
            return;

        if (restoreTimeScale)
            Time.timeScale = 1f;

        transitionRequested = manager.BeginTransition(startScreenName);
    }
}
