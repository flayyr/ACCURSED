using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsOptionButton : SettingsMenuSelectable
{
    [Header("Button Action")]
    public UnityEvent onPressed;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public override void Activate()
    {
        if (onPressed != null)
            onPressed.Invoke();

        if (button != null)
            button.onClick.Invoke();
    }
}