using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsKeybindValueArea : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SettingsKeybindRow keybindRow;

    private void Awake()
    {
        if (keybindRow == null)
            keybindRow = GetComponentInParent<SettingsKeybindRow>();
    }

    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (keybindRow == null)
            keybindRow = GetComponentInParent<SettingsKeybindRow>();

        if (keybindRow != null)
            keybindRow.BeginListeningFromValueField();
    }
}