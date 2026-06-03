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

    private Queue<ItemPickupSO> items = new Queue<ItemPickupSO>();

    private bool promptOpen;
    private string promptText;


    private Action currentAction;

    //private float basePopUpYPos = -144f; // y pos of normal item pickup ui popup

    void Awake()
    {
        promptOpen = false;
    }
    public void PromptAppear()
    {
        promptOpen = true;
        GameObject promptIns = Instantiate(UIPromptPrefab);
    }

    public void PromptDisappear() 
    {
        promptOpen = false;
        UIPromptPrefab.SetActive(false);
    }

    private void CheckPromptTrigger()
    {
        if (promptOpen && Input.GetKeyDown(KeyCode.X))
        {
            currentAction?.Invoke();
            currentAction = null;
            PromptDisappear();
        }
    }

    // Normal interaction tooltip
    public void Prompt(string promptText)
    {
        this.promptText = promptText;

        currentAction = () =>
        {
            Debug.Log("Interacted");
        };
    }

    // Multiple item pickup (can only be used for normal items)
    public void Prompt(string PromptText, List<ItemPickupSO> items)
    {
        promptText = PromptText;

        currentAction = () =>
        {
            foreach (ItemPickupSO item in items)
            {
                UINormalItemPrefab.GetComponent<NormalItemPickup>().AddItem(item);
            }
        };
        
    }

    // Singular item pickup, normal or special item
    public void Prompt(string PromptText, ItemPickupSO item) 
    {
        promptText = PromptText;

        currentAction = () =>
        {
            if (item.isSpecialItem)
            {
                UISpecialItemPrefab.GetComponent<SpecialItemPickup>().AddItem(item);
            }
            else
            {
                UINormalItemPrefab.GetComponent<NormalItemPickup>().AddItem(item);
            }
        };
    }


    public void Update()
    {
        CheckPromptTrigger();

        if (Input.GetKeyDown(KeyCode.Alpha1)) // debug command, normally triggered by if you approach a point too close
        {
            PromptAppear();
            Prompt("Loot", debugList);
        }

        // if (too far away) { PromptDisappear }
    }


}
