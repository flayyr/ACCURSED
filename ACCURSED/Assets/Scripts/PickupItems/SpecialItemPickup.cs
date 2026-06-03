using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Progress;

public class SpecialItemPickup : ItemPickup
{
    [SerializeField] GameObject UIPromptPrefab;
    [SerializeField] GameObject UISpecialItemPickup;

    public Queue<ItemPickupSO> itemPickupQueue;

    private ItemPickupSO specialItem;
    void Awake()
    {

    }

    public void Update()
    {

    }

    public override void AddItem(ItemPickupSO item)
    {
        item = specialItem;

        // instantiate "OK" prompt
        GameObject pickupConf = Instantiate(UIPromptPrefab);
        PromptUI promptProperties = pickupConf.GetComponent<PromptUI>();

        promptProperties.curScenario = PromptUI.Scenario.ItemCollect;
        promptProperties.promptText = "OK";

        // instantiate normal item showcase
        GameObject itemShowcase = Instantiate(UISpecialItemPickup);
        NormalItemPickupUI normalItemShowcaseProperties = itemShowcase.GetComponent<NormalItemPickupUI>();

        // assign to item
        item.promptUIObj = pickupConf;
        item.itemShowcaseUIObj = itemShowcase;
    }

    public override void ConfirmItem()
    {

        // pseudocode: add item to inventory

        Destroy(specialItem.promptUIObj);
        Destroy(specialItem.itemShowcaseUIObj);

    }
}
