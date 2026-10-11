using System;
using UnityEngine;

//attach this script to all interactable objects (aspects, chests, etc)
public class InteractableObjectManager : MonoBehaviour
{
    [SerializeField] private InteractableItemSO obj; // attach respective item.

    //[SerializeField] private GameObject player;
    [SerializeField] private string promptText;

    protected bool isInsideTrigger;

    public static bool promptOpen = false;

    public Action OnPromptTriggered;

    void Awake()
    {
        
    }

    protected virtual void Update()
    {
        if (isInsideTrigger && !GlobalUIController.Instance.CheckIfOtherUIOpen() && !promptOpen)
        {
            OpenPrompt();
        }

        //Debug.Log(isInsideTrigger + "POOP");
        //Debug.Log(GlobalUIController.Instance.CheckIfOtherUIOpen() + "POOOOP");
        //Debug.Log(!promptOpen + "POOOOOOOOOOOOOP");
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        isInsideTrigger = true;
        /*if (!promptOpen)
        {
            OpenPrompt();
        }*/
       
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        isInsideTrigger = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        isInsideTrigger = false;
        promptOpen = false;
        ToolTipManager.Instance.ManuallyRemovePrompt();
    }

    protected void OpenPrompt()
    {
        //Debug.Log("prompt appear");
        promptOpen = true;
        ToolTipManager.Instance.Prompt(promptText, obj, OnPromptTriggered);
    }

}
