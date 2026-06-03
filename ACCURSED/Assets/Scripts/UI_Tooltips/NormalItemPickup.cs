using UnityEngine;
using System.Collections.Generic;

public class NormalItemPickup : ItemPickup
{
    [SerializeField] GameObject UIPromptPrefab;
    [SerializeField] GameObject UINormalItemPickup;

    public Queue<ItemPickupSO> itemPickupQueue;
    void Awake()
    {
        
    }

    public void Update()
    {
     
    }
    public override void AddItem(ItemPickupSO item)
    {
        itemPickupQueue.Enqueue(item);

        // instantiate "OK" prompt
        GameObject pickupConf = Instantiate(UIPromptPrefab);
        PromptUI promptProperties = pickupConf.GetComponent<PromptUI>();

        promptProperties.promptText = "OK";

        // instantiate normal item showcase
        GameObject itemShowcase = Instantiate(UINormalItemPickup);
        NormalItemPickupUI normalItemShowcaseProperties = itemShowcase.GetComponent<NormalItemPickupUI>();

        // assign to item
        item.promptUIObj = pickupConf;
        item.itemShowcaseUIObj = itemShowcase;

    }

    public override void ConfirmItem()
    {
        ItemPickupSO item = itemPickupQueue.Dequeue();

        // pseudocode: add item to inventory

        Destroy(item.promptUIObj);
        Destroy(item.itemShowcaseUIObj);

    }
}
