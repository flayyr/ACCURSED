using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    [SerializeField] GameObject promptTextUI;
    [SerializeField] NormalItemPickup normalItemPickup;

    public string promptText;

    void Awake()
    {

    }


    void Update()
    {
        promptTextUI.GetComponent<TextMeshProUGUI>().text = "[X] " + promptText;

    }
}
