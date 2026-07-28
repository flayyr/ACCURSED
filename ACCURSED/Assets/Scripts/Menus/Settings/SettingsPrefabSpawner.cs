using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsPrefabSpawner : MonoBehaviour
{
    [Header("Settings Prefab")]
    public GameObject settingsPrefab;

    [Tooltip("Optional. If empty, this script will find the first Canvas in the scene.")]
    public Canvas targetCanvas;

    [Header("Screen Scaling")]
    [Tooltip("Automatically configure the Canvas Scaler for different screen sizes.")]
    [SerializeField] private bool configureCanvasScaler = true;

    [Tooltip("The resolution at which the settings UI was originally designed.")]
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);

    [Tooltip("0 matches width, 1 matches height, and 0.5 balances both.")]
    [SerializeField, Range(0f, 1f)]
    private float matchWidthOrHeight = 0.5f;

    [Tooltip("Useful for pixel-art UI, but may not be appropriate for every project.")]
    [SerializeField] private bool canvasPixelPerfect = false;

    [Header("Behavior")]
    public bool closeWithEscape = true;
    public bool destroyOnClose = true;

    [Tooltip("Objects to temporarily hide while settings is open.")]
    public List<GameObject> objectsToDisableWhileOpen = new List<GameObject>();

    [Tooltip("Scripts to temporarily disable while settings is open, like StartMenuManager.")]
    public List<MonoBehaviour> scriptsToDisableWhileOpen = new List<MonoBehaviour>();

    [Header("Start Menu Input")]
    [SerializeField] private StartMenuManager startMenuManager;
    [SerializeField] private string playerTag = "Player";

    private GameObject settingsInstance;
    private SettingsMenuNavigator settingsNavigator;
    private bool isOpen;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    private void Awake()
    {
        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        if (startMenuManager == null)
            startMenuManager = FindFirstObjectByType<StartMenuManager>();

        FindTargetCanvas();
        ConfigureCanvasScaling();
    }

    private void Update()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            CharacterMovement scriptToDisable = playerObject.GetComponent<CharacterMovement>();

            Debug.Log("Time.timeScale = " + Time.timeScale);
            Debug.Log("ScriptToDisable = " + scriptToDisable.enabled);
            Debug.Log("PlayerInput = " + playerObject.GetComponent<PlayerInput>().enabled);

            if (!isOpen)
            {
                scriptToDisable.enabled = true;
                playerObject.GetComponent<PlayerInput>().enabled = true;
            }

            if (isOpen)
            {
                scriptToDisable.enabled = false;
                playerObject.GetComponent<PlayerInput>().enabled = false;
            }
        }

        if (!isOpen)
        {
            Time.timeScale = 1f;
            //EnableSceneObjects();
            return;
        }

        if (isOpen)
        {
            Time.timeScale = 0f;
            DisableSceneObjects();
        }
            

        if (!closeWithEscape)
            return;

        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseSettings();

        // Escape is currently being assigned as a keybind,
        // or was assigned during this same frame.
        if (settingsNavigator != null)
        {
            return;
        }


        CloseSettings();

        /*
        if (isOpen && closeWithEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            // While rebinding, Escape is treated as the new key.
            // It must not close the settings menu.
            if (settingsNavigator != null && settingsNavigator.IsListeningForBinding)
                return;

            // During slider adjustment, Escape exits adjustment instead of closing the entire menu.
            if (settingsNavigator != null && settingsNavigator.IsAdjustingSlider)
            {
                settingsNavigator.StopSliderAdjustMode();
                return;
            }

            CloseSettings();
        }
        */
    }

    public void OpenSettings()
    {
        if (settingsPrefab == null)
        {
            Debug.LogWarning("SettingsPrefabSpawner has no settings prefab assigned.", this);
            return;
        }

        if (targetCanvas == null)
        {
            FindTargetCanvas();

            if (targetCanvas == null)
            {
                Debug.LogWarning("No Canvas found in the scene.", this);
                return;
            }

            ConfigureCanvasScaling();
        }

        if (isOpen)
            return;

        isOpen = true;

        if (startMenuManager != null)
            startMenuManager.SetSettingsInputBlocked(true);

        SpawnSettingsPrefab();

        if (settingsInstance != null)
            settingsInstance.SetActive(true);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (settingsNavigator != null)
        {
            settingsNavigator.ResetNavigation();
        }
        else
        {
            Debug.LogWarning("Spawned settings prefab does not contain a SettingsMenuNavigator.", settingsInstance);
        }
    }

    public void CloseSettings()
    {
        if (!isOpen)
            return;

        if (settingsNavigator != null)
        {
            if (settingsNavigator.IsListeningForBinding)
                settingsNavigator.CancelKeybindListen();

            if (settingsNavigator.IsAdjustingSlider)
                settingsNavigator.StopSliderAdjustMode();
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (settingsInstance != null)
        {
            // Disable immediately so the two menus cannot both
            // receive input during the remainder of this frame.
            settingsInstance.SetActive(false);

            if (destroyOnClose)
            {
                Destroy(settingsInstance);
                settingsInstance = null;
                settingsNavigator = null;
            }
        }

        isOpen = false;

        if (startMenuManager != null)
            startMenuManager.SetSettingsInputBlocked(false);

        EnableSceneObjects();
    }

    public void ToggleSettings()
    {
        if (isOpen)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    private void FindTargetCanvas()
    {
        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();
    }

    private void ConfigureCanvasScaling()
    {
        if (!configureCanvasScaler || targetCanvas == null)
            return;

        CanvasScaler canvasScaler = targetCanvas.GetComponent<CanvasScaler>();

        if (canvasScaler == null)
            canvasScaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = referenceResolution;

        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        canvasScaler.matchWidthOrHeight = matchWidthOrHeight;

        targetCanvas.pixelPerfect = canvasPixelPerfect;
    }

    private void SpawnSettingsPrefab()
    {
        if (settingsInstance == null)
        {
            settingsInstance = Instantiate(settingsPrefab, targetCanvas.transform, false);

            // Remove "(Clone)" from its runtime name if desired.
            settingsInstance.name = settingsPrefab.name;

            // The instantiated object is already the settings root.
            RectTransform settingsRect = settingsInstance.GetComponent<RectTransform>();

            if (settingsRect != null)
            {
                settingsRect.anchorMin = Vector2.zero;
                settingsRect.anchorMax = Vector2.one;
                settingsRect.offsetMin = Vector2.zero;
                settingsRect.offsetMax = Vector2.zero;
                settingsRect.localScale = Vector3.one;
            }

            settingsNavigator = settingsInstance.GetComponentInChildren<SettingsMenuNavigator>(true);

            if (settingsNavigator == null)
                Debug.LogWarning( "No SettingsMenuNavigator was found inside " + settingsInstance.name, settingsInstance);
        }
    }
    
    private void DisableSceneObjects()
    {
        foreach (GameObject obj in objectsToDisableWhileOpen)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (MonoBehaviour script in scriptsToDisableWhileOpen)
        {
            if (script != null)
                script.enabled = false;
        }
    }

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindChildRecursive(child, childName);

            if (result != null)
                return result;
        }

        return null;
    }

    private void EnableSceneObjects()
    {
        foreach (GameObject obj in objectsToDisableWhileOpen)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (MonoBehaviour script in scriptsToDisableWhileOpen)
        {
            if (script != null)
                script.enabled = true;
        }
    }
}