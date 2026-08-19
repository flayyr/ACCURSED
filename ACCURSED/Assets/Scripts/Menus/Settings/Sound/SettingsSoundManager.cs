using System;
using System.Collections.Generic;
using UnityEngine;

public enum SettingsSoundSetting
{
    MasterVolume,
    MusicVolume,
    SoundEffectsVolume,
    AmbienceVolume,
    UIVolume
}

public class SettingsSoundManager : MonoBehaviour
{
    [Header("Saving")]
    [Tooltip("Stores sound settings between sessions.")]
    [SerializeField] private bool saveWithPlayerPrefs = true;

    [SerializeField] private string playerPrefsPrefix = "SettingsSound_";

    private readonly Dictionary<SettingsSoundSetting, float> currentValues = new Dictionary<SettingsSoundSetting, float>();

    private readonly List<SettingsSoundRow> registeredRows = new List<SettingsSoundRow>();

    private bool initialized;

    /// <summary>
    /// Other audio scripts can subscribe to this later.
    /// The float is always between 0 and 1.
    /// </summary>
    public event Action<SettingsSoundSetting, float> SettingChanged;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnDisable()
    {
        if (saveWithPlayerPrefs)
            PlayerPrefs.Save();
    }

    public float GetValue(SettingsSoundSetting setting)
    {
        EnsureInitialized();

        if (currentValues.TryGetValue(setting, out float value))
            return value;

        return GetDefaultValue(setting);
    }

    public void SetValue(SettingsSoundSetting setting, float value)
    {
        EnsureInitialized();

        value = Mathf.Clamp01(value);

        currentValues[setting] = value;

        if (saveWithPlayerPrefs)
            PlayerPrefs.SetFloat(GetSaveKey(setting), value);

        SettingChanged?.Invoke(setting, value);

        RefreshAllRows();
    }

    public void RestoreDefaults()
    {
        EnsureInitialized();

        foreach (SettingsSoundSetting setting
                 in Enum.GetValues(typeof(SettingsSoundSetting)))
        {
            float defaultValue = GetDefaultValue(setting);

            currentValues[setting] = defaultValue;

            if (saveWithPlayerPrefs)
            {
                PlayerPrefs.SetFloat(GetSaveKey(setting), defaultValue);
            }

            SettingChanged?.Invoke(setting, defaultValue);
        }

        if (saveWithPlayerPrefs)
            PlayerPrefs.Save();

        RefreshAllRows();
    }

    public void RegisterRow(SettingsSoundRow row)
    {
        EnsureInitialized();

        if (row == null)
            return;

        if (!registeredRows.Contains(row))
            registeredRows.Add(row);

        row.RefreshDisplay();
    }

    public void UnregisterRow(SettingsSoundRow row)
    {
        registeredRows.Remove(row);
    }

    private void RefreshAllRows()
    {
        for (int i = registeredRows.Count - 1; i >= 0; i--)
        {
            SettingsSoundRow row = registeredRows[i];

            if (row == null)
            {
                registeredRows.RemoveAt(i);
                continue;
            }

            row.RefreshDisplay();
        }
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        currentValues.Clear();

        foreach (SettingsSoundSetting setting
                 in Enum.GetValues(typeof(SettingsSoundSetting)))
        {
            float defaultValue =
                GetDefaultValue(setting);

            currentValues[setting] =
                LoadValue(setting, defaultValue);
        }

        initialized = true;
    }

    private float LoadValue(SettingsSoundSetting setting, float defaultValue)
    {
        if (!saveWithPlayerPrefs)
            return defaultValue;

        string saveKey =
            GetSaveKey(setting);

        if (!PlayerPrefs.HasKey(saveKey))
            return defaultValue;

        return Mathf.Clamp01(PlayerPrefs.GetFloat(saveKey, defaultValue));
    }

    private string GetSaveKey(
        SettingsSoundSetting setting)
    {
        return playerPrefsPrefix + setting;
    }

    public float GetDefaultValue(
        SettingsSoundSetting setting)
    {
        switch (setting)
        {
            case SettingsSoundSetting.MasterVolume:
                return 1f;

            case SettingsSoundSetting.MusicVolume:
                return 1f;

            case SettingsSoundSetting.SoundEffectsVolume:
                return 1f;

            case SettingsSoundSetting.AmbienceVolume:
                return 1f;

            case SettingsSoundSetting.UIVolume:
                return 1f;
        }

        return 1f;
    }

    public string GetDisplayName(
        SettingsSoundSetting setting)
    {
        switch (setting)
        {
            case SettingsSoundSetting.MasterVolume:
                return "Master Volume";

            case SettingsSoundSetting.MusicVolume:
                return "Music Volume";

            case SettingsSoundSetting.SoundEffectsVolume:
                return "Sound Effects";

            case SettingsSoundSetting.AmbienceVolume:
                return "Ambience";

            case SettingsSoundSetting.UIVolume:
                return "UI Volume";
        }

        return setting.ToString();
    }
}