using UnityEngine;
using System.Collections.Generic;

public class NormalItemPickup : ItemPickup
{
    [SerializeField] GameObject UIPromptPrefab;
    [SerializeField] GameObject UINormalItemPickup;

    public Queue<ItemPickupSO> itemPickupQueue = new Queue<ItemPickupSO>();
    void Awake()
    {
        
    }

    public void Update()
    {
        if (itemPickupQueue.Count > 0 && Input.GetKeyDown(KeyCode.X))
        {
            ConfirmItem();
        }
    }
    public override void AddItem(ItemPickupSO item)
    {
        itemPickupQueue.Enqueue(item);

        // instantiate "OK" prompt
        GetComponent<ToolTipManager>().Prompt("OK");

        // instantiate normal item showcase
        GameObject itemShowcase = Instantiate(UINormalItemPickup);

        // assign to item
        item.itemShowcaseUIObj = itemShowcase; ;

    }

    public override void ConfirmItem()
    {
        ItemPickupSO item = itemPickupQueue.Dequeue();

        // pseudocode: add item to inventory
        Destroy(item.itemShowcaseUIObj);

    }
}
