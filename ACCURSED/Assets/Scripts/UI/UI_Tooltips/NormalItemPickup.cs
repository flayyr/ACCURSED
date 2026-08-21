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
        Debug.Log("NormalItemPickup.AddItem called: " + item.itemName);

        itemPickupQueue.Enqueue(item);

        //GetComponent<ToolTipManager>().Prompt("OK");

        if (UINormalItemPickup == null)
        {
            Debug.LogError("NormalItemPickup: UINormalItemPickup prefab is NULL.");

            return;
        }

        if (container == null)
        {
            Debug.LogError("NormalItemPickup: container is NULL.");

            return;
        }


        Debug.Log("NormalItemPickup: instantiating UI_NormalItemPickup.");
        
        GameObject itemShowcase = Instantiate(UINormalItemPickup, container, false);

        itemShowcase.transform.SetAsFirstSibling();

        addedNewObjectThisUpdate = true;
        
        NormalItemPickupUI ui = itemShowcase.GetComponent<NormalItemPickupUI>();

        if (ui == null)
        {
            Debug.LogError("UI_NormalItemPickup does not have " + "NormalItemPickupUI attached.");

            return;
        }

        ui.Initialize(item);
        ui.manager = this;
        
        item.itemShowcaseUIObj = itemShowcase;
        
        Debug.Log("NormalItemPickup: UI successfully created.");
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
