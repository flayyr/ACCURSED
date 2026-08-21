using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

// This class manages the overall Item Pickup UI System 
public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance { get; private set; }

    [SerializeField] Transform parentCanvas;
    [SerializeField] GameObject UIPromptPrefab;
    [SerializeField] GameObject UINormalItemPrefab;
    [SerializeField] GameObject UISpecialItemPrefab;

    [SerializeField] GameObject toolTipPrefab;

    public List<ItemPickupSO> debugList; //debug
    public ItemPickupSO debugItem; //debug

    private Queue<ItemPickupSO> items = new Queue<ItemPickupSO>();

    private static bool promptOpen;
    public bool IsPromptOpen => promptOpen;
    private string promptText;

    [SerializeField] private CanvasGroup promptCanvas;
    private Coroutine menuAppear;


    private Action currentAction;
    private GameObject currentPrompt;

    void Awake()
    {
        promptOpen = false;

        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //private void OnEnable()
    //{
    //    if(PersistentPlayer.Instance != null)
    //    PersistentPlayer.Instance.GetComponent<PlayerController>().InteractKeyPressed += CheckPromptTrigger;
    //}
    private void Start()
    {
        //for some reason onEnable gets called before persistent player instance is set sometimes, assigning again in start to avoid that.
        if (PersistentPlayer.Instance != null)
            PersistentPlayer.Instance.GetComponent<PlayerController>().InteractKeyPressed += CheckPromptTrigger;
    }
    private void OnDisable()
    {
        if(PersistentPlayer.Instance!=null)
        PersistentPlayer.Instance.GetComponent<PlayerController>().InteractKeyPressed -= CheckPromptTrigger;
    }

    public void PromptAppear()
    {
        
        if (promptOpen) { return; }

        if (GlobalUIController.Instance.CheckIfOtherUIOpen()) { return; }

        StopCurrentTransition();
        promptOpen = true;

        currentPrompt = Instantiate(UIPromptPrefab, parentCanvas, false);
        RectTransform rt = currentPrompt.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 280);

        PromptUI ui = currentPrompt.GetComponent<PromptUI>();
        ui.SetText(promptText);

        menuAppear = StartCoroutine(MenuOpenRoutine());
    }

    public void PromptDisappear()
    {
        StopCurrentTransition();

        InteractableObjectManager.promptOpen = false;
        promptOpen = false;

        menuAppear = StartCoroutine(MenuCloseRoutine());
    }

    private IEnumerator MenuOpenRoutine()
    {

        promptCanvas.alpha = 0f;
        yield return UITransitions.Instance.FadeTransition(promptCanvas, 0f, 1f, 0.1f);
    }

    private IEnumerator MenuCloseRoutine()
    {
        promptCanvas.alpha = 1f;
        yield return UITransitions.Instance.FadeTransition(promptCanvas, 1f, 0f, 0.1f);

        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
            currentPrompt = null;
        }
    }

    private void CheckPromptTrigger()
    {
        if (promptOpen)
        {
            if (GetComponent<NormalItemPickup>().itemPickupQueue.Count < 2)
            {
                PromptDisappear();
            }
            currentAction?.Invoke();
            currentAction = null;
        }
    }

    public void ManuallyRemovePrompt() // if prompt interaction goes out of bounds
    {
        if (promptOpen)
        {
            PromptDisappear();
        }
        currentAction = null;
    }

    // prompt popup, no action
    public void Prompt(string promptText)
    {
        this.promptText = promptText;
        PromptAppear();
    }

    // Normal interaction tooltip
    public void Prompt(string promptText, InteractableItemSO obj)
    {
        this.promptText = promptText;
        PromptAppear();

        currentAction = () =>
        {
            obj.Interact();
        };
    }

    // Multiple item pickup (can only be used for normal items). Uses dictionary for item stacking (see StackItems)
    public void Prompt(string promptText, List<ItemPickupSO> items)
    {
        this.promptText = promptText;
        PromptAppear();

        currentAction = () =>
        {
            var stackedItems = StackItems(items);
            foreach (var entry in stackedItems)
            {
                StackedItem stackedItem = entry.Value;

                ItemPickupSO newItem =
                    ScriptableObject.CreateInstance<ItemPickupSO>();

                newItem.itemName = stackedItem.itemName;
                newItem.itemQuantity = stackedItem.itemQuantity;
                newItem.itemSpr = stackedItem.itemSpr;

                GetComponent<NormalItemPickup>().AddItem(newItem);
            }
        };
    }

    // Singular item pickup, normal or special item
    public void Prompt(string promptText, ItemPickupSO item)
    {
        Prompt(promptText, item, null);
    }


    // Singular item pickup with callback
    public void Prompt(string promptText, ItemPickupSO item, Action onPickedUp)
    {
        this.promptText = promptText;
        PromptAppear();

        currentAction = () =>
        {
            // Tell the world item it has been picked up FIRST.
            onPickedUp?.Invoke();

            // Then handle its pickup UI.
            if (item.isSpecialItem)
            {
                GetComponent<SpecialItemPickup>().AddItem(item);
            }
            else
            {
                GetComponent<NormalItemPickup>().AddItem(item);
            }
        };
    }

    public void Update()
    {
        ToolTipDebug(); //DEBUG

        // if (too far away) { PromptDisappear }
    }

    private void ToolTipDebug() 
    {
       
        /* debug commands, normally triggered by if you approach a point too close. 
         * 1 = regular interactable tooltip
         * 2 = to loot multiple normal items
         * 3 = to loot a singular special item */

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToolTipManager.Instance.Prompt("Confirm");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToolTipManager.Instance.Prompt("Loot", debugList);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToolTipManager.Instance.Prompt("Loot", debugItem);
        }
    }

    // Used for stacking multiple normal items for cleaner UI
    private Dictionary<string, StackedItem> StackItems(List<ItemPickupSO> items)
    {
        Dictionary<string, StackedItem> stacked = new();

        foreach (ItemPickupSO item in items)
        {
            if (stacked.ContainsKey(item.itemName))
            {
                stacked[item.itemName].itemQuantity += item.itemQuantity;
            }
            else
            {
                stacked[item.itemName] = new StackedItem
                {
                    itemName = item.itemName,
                    itemQuantity = item.itemQuantity,
                    itemSpr = item.itemSpr
                };
            }
        }

        return stacked;
    }

    private void StopCurrentTransition()
    {
        if (menuAppear != null)
        {
            StopCoroutine(menuAppear);
            menuAppear = null;
        }
    }


}
