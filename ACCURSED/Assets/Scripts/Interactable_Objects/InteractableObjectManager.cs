using UnityEngine;

//attach this script to all interactable objects (aspects, chests, etc)
public class InteractableObjectManager : MonoBehaviour
{
    [SerializeField] private InteractableItemSO obj; // attach respective item.

    [SerializeField] private GameObject player;
    [SerializeField] private string promptText;

    private bool promptOpen = false;

    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("prompt appear");
        promptOpen = true;
        ToolTipManager.Instance.Prompt(promptText, obj);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        promptOpen = false;
        ToolTipManager.Instance.ManuallyRemovePrompt();
    }
}
