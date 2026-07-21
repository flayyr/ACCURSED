using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [Header("Settings Menu Size")]
    [Tooltip("Name of the child object containing the visible settings menu.")]
    [SerializeField] private string settingsPanelName = "SettingsPanel";

    [Tooltip("Additional size multiplier for the visible settings menu.")]
    [SerializeField, Min(0.1f)] private float settingsPanelScale = 1.25f;

    private GameObject settingsInstance;
    private SettingsMenuNavigator settingsNavigator;
    private bool isOpen;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    private void Awake()
    {
        FindTargetCanvas();
        ConfigureCanvasScaling();
    }

    private void Update()
    {
        if (isOpen && closeWithEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            // While rebinding, Escape is treated as the new key.
            // It must not close the settings menu.
            if (settingsNavigator != null &&
                settingsNavigator.IsListeningForBinding)
            {
                return;
            }

            // During slider adjustment, Escape exits adjustment
            // instead of closing the entire menu.
            if (settingsNavigator != null &&
                settingsNavigator.IsAdjustingSlider)
            {
                settingsNavigator.StopSliderAdjustMode();
                return;
            }

            CloseSettings();
        }
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

        DisableSceneObjects();
        SpawnSettingsPrefab();

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

        isOpen = false;

        if (settingsNavigator != null && settingsNavigator.IsAdjustingSlider)
            settingsNavigator.StopSliderAdjustMode();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (settingsInstance != null)
        {
            if (destroyOnClose)
            {
                Destroy(settingsInstance);

                settingsInstance = null;
                settingsNavigator = null;
            }
            else
            {
                settingsInstance.SetActive(false);
            }
        }

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
            settingsInstance = Instantiate(settingsPrefab, targetCanvas.transform, false
            );

            // Keep the root stretched across the entire screen.
            RectTransform rootRect = settingsInstance.GetComponent<RectTransform>();

            if (rootRect != null)
            {
                rootRect.anchorMin = Vector2.zero;
                rootRect.anchorMax = Vector2.one;

                rootRect.offsetMin = Vector2.zero;
                rootRect.offsetMax = Vector2.zero;

                rootRect.anchoredPosition = Vector2.zero;
                rootRect.localScale = Vector3.one;
                rootRect.localRotation = Quaternion.identity;
            }

            // Scale the visible menu panel rather than the full-screen root.
            Transform settingsPanel = FindChildRecursive(settingsInstance.transform, settingsPanelName);

            if (settingsPanel != null)
            {
                settingsPanel.localScale = Vector3.one * settingsPanelScale;
            }
            else
            {
                Debug.LogWarning("Could not find a child named " + settingsPanelName + 
                    " inside the SettingsPrefab.", settingsInstance);
            }

            settingsNavigator =
                settingsInstance.GetComponentInChildren
                <SettingsMenuNavigator>(true);
        }
        else
        {
            settingsInstance.SetActive(true);
        }

        settingsInstance.transform.SetAsLastSibling();
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