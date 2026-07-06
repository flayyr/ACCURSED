using UnityEngine;
using System.Collections.Generic;

public class NormalItemPickup : ItemPickup
{
    [SerializeField] GameObject UIPromptPrefab;
    [SerializeField] GameObject UINormalItemPickup;
    [SerializeField] private Transform container;

    public Queue<ItemPickupSO> itemPickupQueue = new Queue<ItemPickupSO>();
    bool addedNewObjectThisUpdate = false;


    void Awake()
    {
        
    }

    public void Update()
    {
        if (itemPickupQueue.Count > 0 && Input.GetKeyDown(KeyCode.E) && !addedNewObjectThisUpdate)
        {
            ConfirmItem();
        }


        //keep this at the bottom of update, it stops the new object from getting deleted the same frame it's added
        if (addedNewObjectThisUpdate)
        {
            addedNewObjectThisUpdate = false;
        }
    }
    public override void AddItem(ItemPickupSO item)
    {
        itemPickupQueue.Enqueue(item);

        // instantiate "OK" prompt
        GetComponent<ToolTipManager>().Prompt("OK");

        // instantiate normal item showcase

        GameObject itemShowcase = Instantiate(UINormalItemPickup, container, false);
        itemShowcase.transform.SetAsFirstSibling();
        addedNewObjectThisUpdate = true;


        NormalItemPickupUI ui = itemShowcase.GetComponent<NormalItemPickupUI>();
        ui.Initialize(item);
        //ui.item = item;
        ui.manager = this;


        // assign to item
        item.itemShowcaseUIObj = itemShowcase;

    }

    public override void ConfirmItem()
    {
        ItemPickupSO item = itemPickupQueue.Dequeue();

        // pseudocode: add item to inventory

        // transition and destroy
        NormalItemPickupUI ui = item.itemShowcaseUIObj.GetComponent<NormalItemPickupUI>();
        ui.Confirmed();
        ui.inQueue = false;

    }
}
