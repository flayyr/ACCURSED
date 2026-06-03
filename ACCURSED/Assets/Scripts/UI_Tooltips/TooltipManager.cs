using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
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

    private float basePopUpYPos = -144f; // y pos of normal item pickup ui popup

    public void ManagePickupUI()
    {
        
    }

    // Normal interaction tooltip
    public void Prompt(string promptText) 
    {

    }

    // Multiple item pickup (can only be used for normal items)
    public void Prompt(string PromptText, List<ItemPickupSO> items)
    {
        foreach (ItemPickupSO item in items)
        {
            UINormalItemPrefab.GetComponent<NormalItemPickup>().AddItem(item);
        }

    }

    // Singular item pickup, normal or special item
    public void Prompt(string PromptText, ItemPickupSO item) 
    {
        if (item.isSpecialItem)
        {
            UISpecialItemPrefab.GetComponent<SpecialItemPickup>().AddItem(item);
        }
        else
        {
            UISpecialItemPrefab.GetComponent<SpecialItemPickup>().AddItem(item);
        }
    }

    public void Update()
    {
        //ManagePickupUI();



        if (Input.GetKeyDown(KeyCode.Alpha1)) // debug command
        {
            Prompt("Loot", debugList);
        }
    }


}
