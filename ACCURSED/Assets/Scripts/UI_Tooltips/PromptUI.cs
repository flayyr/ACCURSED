using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    [SerializeField] GameObject promptTextUI;
    [SerializeField] NormalItemPickup normalItemPickup;
    public enum Scenario
    {
        Interact, // "Investigate"
        ItemCollect, // "OK"
        Other
    }

    public Scenario curScenario;
    public string promptText;

    void Awake()
    {

    }

    void InteractPrompt()
    {
        switch (curScenario)
        {
            case Scenario.Interact:
                break;
            case Scenario.ItemCollect:
                //normalItemPickup.GetComponent<ItemPickupUI>().ManagePickupUI
                break;
            case Scenario.Other:
                break;
        }
    }

    void Update()
    {
        promptTextUI.GetComponent<TextMeshProUGUI>().text = "[X] " + promptText;

        if (Input.GetKeyDown(KeyCode.X))
        {
            InteractPrompt();
        }
    }
}
