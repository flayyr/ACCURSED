using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SettingsTransitionHandler : MonoBehaviour
{
    [Header("Settings Prefab")]
    public GameObject settingsPrefab;
    public bool enableSettings = false;

    private void Update()
    {
        if (enableSettings)
        {
            settingsPrefab.SetActive(true);
        }
        else if (!enableSettings)
        {
            settingsPrefab.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (enableSettings)
                enableSettings = false;
        }
    }

    public void ToggleSettings()
    {
        if (enableSettings)
        {
            enableSettings = false;
        }
        else
        {
            enableSettings = true;
        }
    }
}
