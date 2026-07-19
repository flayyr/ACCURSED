using UnityEngine;

//attach this script to all interactable objects (aspects, chests, etc)
public class InteractableObjectManager : MonoBehaviour
{
    [SerializeField] private InteractableItemSO obj; // attach respective item.

    [SerializeField] private GameObject player;
    [SerializeField] private string promptText;

    private bool isInsideTrigger;

    public static bool promptOpen = false;

    void Awake()
    {
        
    }

    void Update()
    {
        if (isInsideTrigger && !GlobalUIController.Instance.CheckIfOtherUIOpen() && !promptOpen)
        {
            
            OpenPrompt();
        }

        Debug.Log(isInsideTrigger + "POOP");
        Debug.Log(GlobalUIController.Instance.CheckIfOtherUIOpen() + "POOOOP");
        Debug.Log(!promptOpen + "POOOOOOOOOOOOOP");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInsideTrigger = true;
        /*if (!promptOpen)
        {
            OpenPrompt();
        }*/
       
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isInsideTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInsideTrigger = false;
        promptOpen = false;
        ToolTipManager.Instance.ManuallyRemovePrompt();
    }

    private void OpenPrompt()
    {
        //Debug.Log("prompt appear");
        promptOpen = true;
        ToolTipManager.Instance.Prompt(promptText, obj);
    }

}
