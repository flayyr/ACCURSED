using System;
using System.Collections.Generic;
using UnityEngine;

// STEP ONE: Add the new action in SettingsKeybindAction.
public enum SettingsKeybindAction
{
    MoveForwards,
    MoveBackwards,
    MoveLeft,
    MoveRight,
    Dodge,
    Sprint,
    Walk,
    Attack,
    HeavyAttack,
    Parry,
    Heal,
    UseItem,
    Remembrance,
    Vestige,
    Interact,
    Menu,
    HUD,
    Test // Such as this one!
}

// This is if more keys are needed for the same action (such as ctl + LMB for Heavy Attack)
public enum SettingsKeybindSlot
{
    Modifier,
    Trigger
}

/// <summary>
/// A keyboard/mouse binding.
/// Single-key actions use Modifier = None.
/// Chord actions use Modifier + Trigger, such as Ctrl + Left Mouse.
/// </summary>
[Serializable]
public struct SettingsKeybind
{
    [SerializeField] private KeyCode modifier;
    [SerializeField] private KeyCode trigger;

    public KeyCode Modifier
    {
        get { return modifier; }
    }

    public KeyCode Trigger
    {
        get { return trigger; }
    }

    public bool IsChord
    {
        get { return modifier != KeyCode.None; }
    }

    public bool IsValid
    {
        get { return trigger != KeyCode.None; }
    }

    public SettingsKeybind(KeyCode trigger)
    {
        modifier = KeyCode.None;
        this.trigger = trigger;
    }

    public SettingsKeybind(KeyCode modifier, KeyCode trigger)
    {
        this.modifier = modifier;
        this.trigger = trigger;
    }

    public KeyCode GetKey(SettingsKeybindSlot slot)
    {
        return slot == SettingsKeybindSlot.Modifier
            ? modifier
            : trigger;
    }

    public SettingsKeybind WithKey(SettingsKeybindSlot slot, KeyCode newKey)
    {
        return slot == SettingsKeybindSlot.Modifier
            ? new SettingsKeybind(newKey, trigger)
            : new SettingsKeybind(modifier, newKey);
    }
}

public class SettingsKeybindManager : MonoBehaviour
{
    [Header("Saving")]
    [Tooltip("Stores the displayed keybind choices between sessions.")]
    [SerializeField] private bool saveWithPlayerPrefs = true;

    [SerializeField] private string playerPrefsPrefix = "SettingsKeybind_";

    private readonly Dictionary<SettingsKeybindAction, SettingsKeybind>
        currentBindings = new Dictionary<SettingsKeybindAction, SettingsKeybind>();

    private readonly List<SettingsKeybindRow> registeredRows = new List<SettingsKeybindRow>();

    private bool initialized;

    private void Awake()
    {
        EnsureInitialized();
    }

    public SettingsKeybind GetBinding(SettingsKeybindAction action)
    {
        EnsureInitialized();

        if (currentBindings.TryGetValue(action, out SettingsKeybind binding))
            return binding;

        return GetDefaultBinding(action);
    }

    public KeyCode GetBindingKey(SettingsKeybindAction action, SettingsKeybindSlot slot)
    {
        return GetBinding(action).GetKey(slot);
    }

    public bool UsesChord(SettingsKeybindAction action)
    {
        return GetBinding(action).IsChord;
    }

    public void SetBinding(SettingsKeybindAction action, SettingsKeybindSlot slot, KeyCode newKey)
    {
        SettingsKeybind currentBinding = GetBinding(action);
        SettingsKeybind updatedBinding = currentBinding.WithKey(slot, newKey);

        SetBinding(action, updatedBinding);
    }

    /// <summary>
    /// Compatibility overload for older code.
    /// Changes the trigger while preserving the modifier.
    /// </summary>
    public void SetBinding(SettingsKeybindAction action, KeyCode newTrigger)
    {
        SetBinding(action, SettingsKeybindSlot.Trigger, newTrigger);
    }

    public void SetBinding(SettingsKeybindAction action, SettingsKeybind newBinding)
    {
        EnsureInitialized();

        currentBindings[action] = newBinding;
        SaveBinding(action, newBinding);
        RefreshAllRows();
    }

    public void RestoreDefaults()
    {
        EnsureInitialized();

        foreach (SettingsKeybindAction action in Enum.GetValues(typeof(SettingsKeybindAction)))
        {
            SettingsKeybind defaultBinding = GetDefaultBinding(action);

            currentBindings[action] = defaultBinding;
            SaveBinding(action, defaultBinding, false);
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

    /// <summary>
    /// Optional helper for future gameplay scripts.
    /// A chord becomes true when its modifier is held and its trigger
    /// is pressed during the current frame.
    /// </summary>
    public bool WasBindingPressed(SettingsKeybindAction action)
    {
        SettingsKeybind binding = GetBinding(action);

        if (!binding.IsValid)
            return false;

        bool modifierSatisfied = binding.Modifier == KeyCode.None || GetKeyEquivalent(binding.Modifier);

        return modifierSatisfied && GetKeyDownEquivalent(binding.Trigger);
    }

    /// <summary>
    /// Optional helper for continuous actions, such as movement or sprint.
    /// </summary>
    public bool IsBindingHeld(SettingsKeybindAction action)
    {
        SettingsKeybind binding = GetBinding(action);

        if (!binding.IsValid)
            return false;

        bool modifierSatisfied = binding.Modifier == KeyCode.None || GetKeyEquivalent(binding.Modifier);

        return modifierSatisfied && GetKeyEquivalent(binding.Trigger);
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
            SettingsKeybind defaultBinding = GetDefaultBinding(action);

            currentBindings[action] = LoadBinding(action, defaultBinding);
        }

        initialized = true;
    }

    private SettingsKeybind LoadBinding(SettingsKeybindAction action, SettingsKeybind defaultBinding)
    {
        if (!saveWithPlayerPrefs)
            return defaultBinding;

        string modifierSaveKey = GetModifierSaveKey(action);
        string triggerSaveKey = GetTriggerSaveKey(action);

        KeyCode modifier = defaultBinding.Modifier;
        KeyCode trigger = defaultBinding.Trigger;

        if (PlayerPrefs.HasKey(modifierSaveKey))
            modifier = ReadSavedKey(modifierSaveKey, defaultBinding.Modifier);

        if (PlayerPrefs.HasKey(triggerSaveKey))
        {
            trigger = ReadSavedKey(triggerSaveKey, defaultBinding.Trigger);
        }
        else
        {
            // Migration from the original one-KeyCode save format.
            string legacySaveKey = GetLegacySaveKey(action);

            if (PlayerPrefs.HasKey(legacySaveKey))
                trigger = ReadSavedKey(legacySaveKey, defaultBinding.Trigger);
        }

        return new SettingsKeybind(modifier, trigger);
    }

    private KeyCode ReadSavedKey(string saveKey, KeyCode fallback)
    {
        int savedValue = PlayerPrefs.GetInt(saveKey, (int)fallback);

        return Enum.IsDefined(typeof(KeyCode), savedValue)
                ? (KeyCode)savedValue
                : fallback;
    }

    private void SaveBinding(SettingsKeybindAction action, SettingsKeybind binding, bool saveImmediately = true)
    {
        if (!saveWithPlayerPrefs)
            return;

        PlayerPrefs.SetInt(GetModifierSaveKey(action), (int)binding.Modifier);

        PlayerPrefs.SetInt(GetTriggerSaveKey(action), (int)binding.Trigger);

        if (saveImmediately)
            PlayerPrefs.Save();
    }

    private string GetLegacySaveKey(SettingsKeybindAction action)
    {
        return playerPrefsPrefix + action;
    }

    private string GetModifierSaveKey(SettingsKeybindAction action)
    {
        return playerPrefsPrefix + action + "_Modifier";
    }

    private string GetTriggerSaveKey(SettingsKeybindAction action)
    {
        return playerPrefsPrefix + action + "_Trigger";
    }

    // STEP Two: Using the same format as below, add the new action's default keybinding.
    // case SettingsKeybindAction.[action name (the same at STEP ONE)]:
    //     return new SettingsKeybind.(KeyCode.[action key]);
    // For action with two keys:
    //     return new SettingsKeybind.(KeyCode.[action key 1], KeyCode.[action key 2]); 
    public SettingsKeybind GetDefaultBinding(SettingsKeybindAction action)
    {
        switch (action)
        {
            case SettingsKeybindAction.MoveForwards:
                return new SettingsKeybind(KeyCode.W);

            case SettingsKeybindAction.MoveBackwards:
                return new SettingsKeybind(KeyCode.S);

            case SettingsKeybindAction.MoveLeft:
                return new SettingsKeybind(KeyCode.A);

            case SettingsKeybindAction.MoveRight:
                return new SettingsKeybind(KeyCode.D);

            case SettingsKeybindAction.Dodge:
                return new SettingsKeybind(KeyCode.Space);

            case SettingsKeybindAction.Sprint:
                return new SettingsKeybind(KeyCode.LeftShift);

            case SettingsKeybindAction.Walk:
                return new SettingsKeybind(KeyCode.LeftAlt);

            case SettingsKeybindAction.Attack:
                return new SettingsKeybind(KeyCode.Mouse0);

            case SettingsKeybindAction.HeavyAttack:
                return new SettingsKeybind(KeyCode.LeftControl, KeyCode.Mouse0);

            case SettingsKeybindAction.Parry:
                return new SettingsKeybind(KeyCode.Mouse1);

            case SettingsKeybindAction.Heal:
                return new SettingsKeybind(KeyCode.Q);

            case SettingsKeybindAction.UseItem:
                return new SettingsKeybind(KeyCode.G);

            case SettingsKeybindAction.Remembrance:
                return new SettingsKeybind(KeyCode.R);

            case SettingsKeybindAction.Vestige:
                return new SettingsKeybind(KeyCode.E);

            case SettingsKeybindAction.Interact:
                return new SettingsKeybind(KeyCode.F);

            case SettingsKeybindAction.Menu:
                return new SettingsKeybind(KeyCode.Escape);

            case SettingsKeybindAction.HUD:
                return new SettingsKeybind(KeyCode.Tab);

            case SettingsKeybindAction.Test: // Such as this one!
                return new SettingsKeybind(KeyCode.T); 
        }

        return new SettingsKeybind(KeyCode.None);
    }

    // STEP THREE: Using the same format as below, add the new action's default name.
    // This is the name shown to the player, and it could be a different name than the one used internally.
    // case SettingsKeybindAction.[action name]:
    //     return "[name to display]";

    public string GetActionDisplayName(SettingsKeybindAction action)
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

            case SettingsKeybindAction.Attack:
                return "Attack";

            case SettingsKeybindAction.HeavyAttack:
                return "Heavy Attack";

            case SettingsKeybindAction.Parry:
                return "Parry";

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

            case SettingsKeybindAction.Test: // Such as this one!
                return "Test";
        }

        return action.ToString();
    }

    public string GetBindingDisplayName(SettingsKeybindAction action)
    {
        SettingsKeybind binding = GetBinding(action);

        if (!binding.IsChord)
            return GetKeyDisplayName(binding.Trigger);

        return GetKeyDisplayName(binding.Modifier) + " + " + GetKeyDisplayName(binding.Trigger);
    }

    // STEP FOUR: If needed, using the same format as below, add the display name for a key.
    // case KeyCode.[KeyCode of that key]:
    //     return "[display name for that key]";
    public string GetKeyDisplayName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.None:
                return "None";

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
                return "Left Mouse Button";

            case KeyCode.Mouse1:
                return "Right Mouse Button";

            case KeyCode.Mouse2:
                return "Middle Mouse Button";

            case KeyCode.Mouse3:
                return "Mouse 4";

            case KeyCode.Mouse4:
                return "Mouse 5";

            case KeyCode.Mouse5:
                return "Mouse 6";

            case KeyCode.Mouse6:
                return "Mouse 7";

            case KeyCode.F24: // Such as this one!
                return "Test Key";
        }

        if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
        {
            int number = (int)key - (int)KeyCode.Alpha0;

            return number.ToString();
        }

        return key.ToString();
    }

    private static bool GetKeyEquivalent(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.LeftControl:
            case KeyCode.RightControl:
                return Input.GetKey(KeyCode.LeftControl) ||
                       Input.GetKey(KeyCode.RightControl);

            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                return Input.GetKey(KeyCode.LeftShift) ||
                       Input.GetKey(KeyCode.RightShift);

            case KeyCode.LeftAlt:
            case KeyCode.RightAlt:
                return Input.GetKey(KeyCode.LeftAlt) ||
                       Input.GetKey(KeyCode.RightAlt);

            default:
                return Input.GetKey(key);
        }
    }

    private static bool GetKeyDownEquivalent(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.LeftControl:
            case KeyCode.RightControl:
                return Input.GetKeyDown(KeyCode.LeftControl) ||
                       Input.GetKeyDown(KeyCode.RightControl);

            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                return Input.GetKeyDown(KeyCode.LeftShift) ||
                       Input.GetKeyDown(KeyCode.RightShift);

            case KeyCode.LeftAlt:
            case KeyCode.RightAlt:
                return Input.GetKeyDown(KeyCode.LeftAlt) ||
                       Input.GetKeyDown(KeyCode.RightAlt);

            default:
                return Input.GetKeyDown(key);
        }
    }
}
