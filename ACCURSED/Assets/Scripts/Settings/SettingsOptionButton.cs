using UnityEngine;
using UnityEngine.Events;

public class SettingsOptionButton : SettingsMenuSelectable
{
    [Header("Button Action")]
    public UnityEvent onPressed;

    public override void Activate()
    {
        onPressed?.Invoke();
    }
}