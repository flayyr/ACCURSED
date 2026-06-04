using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Progress;

public class SpecialItemPickup : ItemPickup
{
    [SerializeField] GameObject UIPromptPrefab;
    [SerializeField] GameObject UISpecialItemPickup;
    [SerializeField] private Transform container;

    public Queue<ItemPickupSO> itemPickupQueue;

    private ItemPickupSO specialItem;
    void Awake()
    {

    }

    public void Update()
    {
        if (specialItem != null && Input.GetKeyDown(KeyCode.X))
        {
            ConfirmItem();
        }
    }

    public override void AddItem(ItemPickupSO item)
    {
        GetComponent<ToolTipManager>().Prompt("OK");

        // instantiate normal item showcase
        GameObject itemShowcase = Instantiate(UISpecialItemPickup, container, false);
        NormalItemPickupUI normalItemShowcaseProperties = itemShowcase.GetComponent<NormalItemPickupUI>(); // this line might be useless but keeping here for security

        // assign to item
        specialItem = item;
        specialItem.itemShowcaseUIObj = itemShowcase;
    }

    public override void ConfirmItem()
    {
        // pseudocode: add item to inventory
        
        Destroy(specialItem.itemShowcaseUIObj);
        specialItem = null;

    }
}
