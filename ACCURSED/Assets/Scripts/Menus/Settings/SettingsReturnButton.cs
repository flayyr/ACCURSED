using UnityEngine;

public class SettingsReturnButton : MonoBehaviour
{
    private SettingsPrefabSpawner settingsSpawner;

    private void Awake()
    {
        FindSpawner();
    }

    private void FindSpawner()
    {
        if (settingsSpawner != null)
            return;

        settingsSpawner = FindFirstObjectByType<SettingsPrefabSpawner>();
    }

    public void CloseSettings()
    {
        FindSpawner();

        if (settingsSpawner == null)
        {
            Debug.LogWarning( name + ": Could not find SettingsPrefabSpawner.");

            return;
        }

        settingsSpawner.CloseSettings();
    }
}