using TMPro;
using UnityEngine;

public class SettingsBindingDisplayRow : MonoBehaviour
{
    [Header("Display Text")]
    [SerializeField] private TMP_Text actionNameText;
    [SerializeField] private TMP_Text bindingValueText;
    [SerializeField] private TMP_Text statusText;

    public void SetDisplay(
        string actionName,
        string bindingValue,
        string status = "")
    {
        if (actionNameText != null)
            actionNameText.text = actionName;

        if (bindingValueText != null)
            bindingValueText.text = bindingValue;

        if (statusText != null)
            statusText.text = status;

        gameObject.name = "Row - " + actionName;
    }
}