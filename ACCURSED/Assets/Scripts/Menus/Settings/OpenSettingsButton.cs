using UnityEngine;

public class OpenSettingsButton : MonoBehaviour
{
    public SettingsPrefabSpawner settingsSpawner;

    public void OpenSettings()
    {
        if (settingsSpawner == null)
        {
            settingsSpawner = FindFirstObjectByType<SettingsPrefabSpawner>();
        }

        if (settingsSpawner == null)
        {
            Debug.LogWarning("No SettingsPrefabSpawner found in the scene.");
            return;
        }

        settingsSpawner.OpenSettings();
    }

    public void ToggleSettings()
    {
        if (settingsSpawner == null)
        {
            settingsSpawner = FindFirstObjectByType<SettingsPrefabSpawner>();
        }

        if (settingsSpawner == null)
        {
            Debug.LogWarning("No SettingsPrefabSpawner found in the scene.");
            return;
        }

        settingsSpawner.ToggleSettings();
    }
}