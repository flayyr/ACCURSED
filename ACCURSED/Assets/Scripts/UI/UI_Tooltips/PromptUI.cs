using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptTextUI;

    public void SetText(string text)
    {
        promptTextUI.text = "[F] " + text;
    }
}