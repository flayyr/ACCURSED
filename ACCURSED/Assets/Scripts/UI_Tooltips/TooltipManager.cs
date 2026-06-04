using UnityEngine;
using System.Collections.Generic;
using System;
using static UnityEditor.Progress;

// This class manages the overall Item Pickup UI System 
public class ToolTipManager : MonoBehaviour
{
    [SerializeField] GameObject UIPromptPrefab;
    [SerializeField] GameObject UINormalItemPrefab;
    [SerializeField] GameObject UISpecialItemPrefab;

    [SerializeField] GameObject toolTipPrefab;

    public List<ItemPickupSO> debugList; //debug
    public ItemPickupSO debugItem; //debug

    private Queue<ItemPickupSO> items = new Queue<ItemPickupSO>();

    private bool promptOpen;
    private string promptText;


    private Action currentAction;
    private GameObject currentPrompt;

    void Awake()
    {
        promptOpen = false;
    }
    public void PromptAppear()
    {
        if (promptOpen) { return; }

        promptOpen = true;
        currentPrompt = Instantiate(UIPromptPrefab);

        PromptUI ui = currentPrompt.GetComponent<PromptUI>();
        ui.SetText(promptText);
    }

    public void PromptDisappear()
    {
        promptOpen = false;

        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
            currentPrompt = null;
        }
    }

    private void CheckPromptTrigger()
    {
        if (promptOpen && Input.GetKeyDown(KeyCode.X))
        {
            if (GetComponent<NormalItemPickup>().itemPickupQueue.Count < 1)
            {
                PromptDisappear();
            }
            currentAction?.Invoke();
            currentAction = null;
        }
    }

    // Normal interaction tooltip
    public void Prompt(string promptText)
    {
        this.promptText = promptText;
        PromptAppear();

        currentAction = () =>
        {
            // Depends on interactable type
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
    public void Prompt(string PromptText, ItemPickupSO item) 
    {
        promptText = PromptText;
        PromptAppear();

        currentAction = () =>
        {
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
        CheckPromptTrigger();

        /* debug commands, normally triggered by if you approach a point too close. 
         * 1 = regular interactable tooltip
         * 2 = to loot multiple normal items
         * 3 = to loot a singular special item */

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            Prompt("Rest");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Prompt("Loot", debugList);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Prompt("Loot", debugItem);
        }

        // if (too far away) { PromptDisappear }
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


}
