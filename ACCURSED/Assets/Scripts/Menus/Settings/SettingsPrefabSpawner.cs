using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsPrefabSpawner : MonoBehaviour
{
    [Header("Settings Prefab")]
    public GameObject settingsPrefab;

    [Tooltip("Optional. If empty, this script will find the first Canvas in the scene.")]
    public Canvas targetCanvas;

    [Header("Behavior")]
    public bool closeWithEscape = true;
    public bool destroyOnClose = true;

    [Tooltip("Objects to temporarily hide while settings is open.")]
    public List<GameObject> objectsToDisableWhileOpen = new List<GameObject>();

    [Tooltip("Scripts to temporarily disable while settings is open, like StartMenuManager.")]
    public List<MonoBehaviour> scriptsToDisableWhileOpen = new List<MonoBehaviour>();

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
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
        }
    }

    private void Update()
    {
        if (!isOpen)
            return;

        if (closeWithEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
        }
    }

    public void OpenSettings()
    {
        if (settingsPrefab == null)
        {
            Debug.LogWarning("SettingsPrefabSpawner has no settings prefab assigned.");
            return;
        }

        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();

            if (targetCanvas == null)
            {
                Debug.LogWarning("No Canvas found in the scene.");
                return;
            }
        }

        if (isOpen)
            return;

        isOpen = true;

        DisableSceneObjects();

        SpawnSettingsPrefab();

        EventSystem.current.SetSelectedGameObject(null);

        if (settingsNavigator != null)
        {
            settingsNavigator.ResetNavigation();
        }
        else
        {
            Debug.LogWarning("Spawned settings prefab does not contain a SettingsMenuNavigator.");
        }
    }

    public void CloseSettings()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (settingsNavigator != null && settingsNavigator.IsAdjustingSlider)
        {
            settingsNavigator.StopSliderAdjustMode();
        }

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

    private void SpawnSettingsPrefab()
    {
        if (settingsInstance == null)
        {
            settingsInstance = Instantiate(settingsPrefab, targetCanvas.transform);

            RectTransform rect = settingsInstance.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            settingsNavigator = settingsInstance.GetComponentInChildren<SettingsMenuNavigator>(true);
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