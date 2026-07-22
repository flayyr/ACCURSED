using System;
using System.Collections.Generic;
using UnityEngine;

public enum SettingsKeybindAction
{
    MoveForwards,
    MoveBackwards,
    MoveLeft,
    MoveRight,
    Dodge,
    Sprint,
    Walk,
    Heal,
    UseItem,
    Remembrance,
    Vestige,
    Interact,
    Menu,
    HUD
}

public class SettingsKeybindManager : MonoBehaviour
{
    [Header("Saving")]
    [Tooltip("Stores the displayed keybind choices between sessions.")]
    [SerializeField] private bool saveWithPlayerPrefs = true;

    [SerializeField] private string playerPrefsPrefix = "SettingsKeybind_";

    private readonly Dictionary<SettingsKeybindAction, KeyCode>
        currentBindings = new Dictionary<SettingsKeybindAction, KeyCode>();

    private readonly List<SettingsKeybindRow> registeredRows = new List<SettingsKeybindRow>();

    private bool initialized;

    private void Awake()
    {
        EnsureInitialized();
    }

    public KeyCode GetBinding(SettingsKeybindAction action)
    {
        EnsureInitialized();

        if (currentBindings.TryGetValue(action, out KeyCode key))
            return key;

        return GetDefaultBinding(action);
    }

    public void SetBinding(SettingsKeybindAction action, KeyCode newKey)
    {
        EnsureInitialized();

        currentBindings[action] = newKey;

        if (saveWithPlayerPrefs)
        {
            PlayerPrefs.SetInt(GetSaveKey(action),(int)newKey);

            PlayerPrefs.Save();
        }

        RefreshAllRows();
    }

    public void RestoreDefaults()
    {
        EnsureInitialized();

        foreach (SettingsKeybindAction action in Enum.GetValues(typeof(SettingsKeybindAction)))
        {
            KeyCode defaultKey = GetDefaultBinding(action);

            currentBindings[action] = defaultKey;

            if (saveWithPlayerPrefs)
            {
                PlayerPrefs.SetInt(GetSaveKey(action), (int)defaultKey);
            }
        }

        if (saveWithPlayerPrefs)
            PlayerPrefs.Save();

        RefreshAllRows();
    }

    public void RegisterRow(SettingsKeybindRow row)
    {
        EnsureInitialized();

        if (row == null)
            return;

        if (!registeredRows.Contains(row))
            registeredRows.Add(row);

        row.RefreshDisplay();
    }

    public void UnregisterRow(SettingsKeybindRow row)
    {
        registeredRows.Remove(row);
    }

    private void RefreshAllRows()
    {
        for (int i = registeredRows.Count - 1; i >= 0; i--)
        {
            SettingsKeybindRow row = registeredRows[i];

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

        currentBindings.Clear();

        foreach (SettingsKeybindAction action in Enum.GetValues(typeof(SettingsKeybindAction)))
        {
            KeyCode defaultKey = GetDefaultBinding(action);
            KeyCode loadedKey = defaultKey;

            string saveKey = GetSaveKey(action);

            if (saveWithPlayerPrefs && PlayerPrefs.HasKey(saveKey))
            {
                loadedKey = (KeyCode)PlayerPrefs.GetInt(saveKey, (int)defaultKey);
            }

            currentBindings[action] = loadedKey;
        }

        initialized = true;
    }

    private string GetSaveKey(SettingsKeybindAction action)
    {
        return playerPrefsPrefix + action;
    }

    public KeyCode GetDefaultBinding(SettingsKeybindAction action)
    {
        switch (action)
        {
            case SettingsKeybindAction.MoveForwards:
                return KeyCode.W;

            case SettingsKeybindAction.MoveBackwards:
                return KeyCode.S;

            case SettingsKeybindAction.MoveLeft:
                return KeyCode.A;

            case SettingsKeybindAction.MoveRight:
                return KeyCode.D;

            case SettingsKeybindAction.Dodge:
                return KeyCode.Space;

            case SettingsKeybindAction.Sprint:
                return KeyCode.LeftShift;

            case SettingsKeybindAction.Walk:
                return KeyCode.LeftAlt;

            case SettingsKeybindAction.Heal:
                return KeyCode.Q;

            case SettingsKeybindAction.UseItem:
                return KeyCode.G;

            case SettingsKeybindAction.Remembrance:
                return KeyCode.R;

            case SettingsKeybindAction.Vestige:
                return KeyCode.E;

            case SettingsKeybindAction.Interact:
                return KeyCode.F;

            case SettingsKeybindAction.Menu:
                return KeyCode.Escape;

            case SettingsKeybindAction.HUD:
                return KeyCode.Tab;
        }

        return KeyCode.None;
    }

    public string GetActionDisplayName(
        SettingsKeybindAction action)
    {
        switch (action)
        {
            case SettingsKeybindAction.MoveForwards:
                return "Move Forwards";

            case SettingsKeybindAction.MoveBackwards:
                return "Move Backwards";

            case SettingsKeybindAction.MoveLeft:
                return "Move Left";

            case SettingsKeybindAction.MoveRight:
                return "Move Right";

            case SettingsKeybindAction.Dodge:
                return "Dodge";

            case SettingsKeybindAction.Sprint:
                return "Sprint";

            case SettingsKeybindAction.Walk:
                return "Walk";

            case SettingsKeybindAction.Heal:
                return "Heal";

            case SettingsKeybindAction.UseItem:
                return "Use Item";

            case SettingsKeybindAction.Remembrance:
                return "Remembrance";

            case SettingsKeybindAction.Vestige:
                return "Vestige";

            case SettingsKeybindAction.Interact:
                return "Interact";

            case SettingsKeybindAction.Menu:
                return "Menu";

            case SettingsKeybindAction.HUD:
                return "HUD";
        }

        return action.ToString();
    }

    public string GetKeyDisplayName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Escape:
                return "Esc";

            case KeyCode.Return:
            case KeyCode.KeypadEnter:
                return "Enter";

            case KeyCode.Space:
                return "Space";

            case KeyCode.Tab:
                return "Tab";

            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                return "Shift";

            case KeyCode.LeftAlt:
            case KeyCode.RightAlt:
                return "Alt";

            case KeyCode.LeftControl:
            case KeyCode.RightControl:
                return "Ctrl";

            case KeyCode.Mouse0:
                return "Left Mouse";

            case KeyCode.Mouse1:
                return "Right Mouse";

            case KeyCode.Mouse2:
                return "Middle Mouse";

            case KeyCode.Mouse3:
                return "Mouse 4";

            case KeyCode.Mouse4:
                return "Mouse 5";

            case KeyCode.Mouse5:
                return "Mouse 6";

            case KeyCode.Mouse6:
                return "Mouse 7";
        }

        if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
        {
            int number = (int)key - (int)KeyCode.Alpha0;

            return number.ToString();
        }

        return key.ToString();
    }
}