using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsRestoreDefaultsButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SettingsKeybindManager keybindManager;
    [SerializeField] private SettingsMenuNavigator menuNavigator;

    // input system
    public bool isPressed { get; }

    private void Awake()
    {
        if (keybindManager == null)
            keybindManager = GetComponentInParent<SettingsKeybindManager>();

        if (menuNavigator == null)
            menuNavigator = GetComponentInParent<SettingsMenuNavigator>();
    }

    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            RestoreDefaults();
        }
    }
    
    public void RestoreDefaults()
    {
        // Nothing else can be clicked while a keybind is waiting
        // for a new input.
        if (menuNavigator != null &&
            menuNavigator.IsListeningForBinding)
        {
            return;
        }

        if (keybindManager != null)
        {
            keybindManager.RestoreDefaults();
        }
    }
}