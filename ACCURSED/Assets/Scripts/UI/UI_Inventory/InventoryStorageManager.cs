using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Progress;

[System.Serializable]
public class InventoryStorage
{
    public Inventory_ItemSO item;
    public Inventory_ItemSlot itemSlot;
    public int quantity;
}

public class InventoryStorageManager : MonoBehaviour
{
    public static InventoryStorageManager Instance { get; private set; }

    private List<InventoryStorage> inventory = new List<InventoryStorage>();

    [SerializeField] private Inventory_ItemSO testItem;

    private List<Inventory_ItemSlot> itemSlots = new List<Inventory_ItemSlot>();

    [SerializeField] private GameObject itemContent;
    [SerializeField] private GameObject itemSlotPrefab;


    void Awake()
    {
        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeItemSlots(40);

        // Debug
        DebugAddTestItems();
    }


    void InitializeItemSlots(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject newItemSlot = Instantiate(itemSlotPrefab, itemContent.transform);

            Inventory_ItemSlot slot = newItemSlot.GetComponent<Inventory_ItemSlot>();

            itemSlots.Add(slot);
        }
    }

    public List<InventoryStorage> GetInventory()
    {
        return inventory;
    }


    public void AddToInventory(Inventory_ItemSO item,int quantity)
    {
        int remainingQuantity = quantity;

        foreach (InventoryStorage storage in inventory)
        {
            if (storage.item != item)
                continue;
            if (storage.quantity >= item.itemQuantityMax)
                continue;


            int spaceLeft = item.itemQuantityMax - storage.quantity;
            int amountToAdd = Mathf.Min(spaceLeft, remainingQuantity);

            storage.quantity += amountToAdd;

            remainingQuantity -= amountToAdd;

            if (remainingQuantity <= 0)
            {
                InitializeInventory();
                return;
            }
        }

        while (remainingQuantity > 0)
        {
            Inventory_ItemSlot emptySlot = GetEmptySlot();

            if (emptySlot == null)
            {
                break;
            }

            int amountToAdd = Mathf.Min(item.itemQuantityMax, remainingQuantity);


            InventoryStorage newStorage = new InventoryStorage {
                item = item,
                itemSlot = emptySlot,
                quantity = amountToAdd };


            inventory.Add(newStorage);

            remainingQuantity -= amountToAdd;
        }


        // Update UI
        InitializeInventory();
    }


    Inventory_ItemSlot GetEmptySlot()
    {
        foreach (Inventory_ItemSlot itemSlot in itemSlots)
        {
            bool slotIsUsed = false;
            foreach (InventoryStorage storage in inventory)
            {
                if (storage.itemSlot == itemSlot)
                {
                    slotIsUsed = true;
                    break;
                }
            }


            if (!slotIsUsed)
            {
                return itemSlot;
            }
        }

        return null;
    }


    public void InitializeInventory()
    {

        foreach (Inventory_ItemSlot itemSlot in itemSlots)
        {
            itemSlot.SetIfEmpty(true);
            itemSlot.SetItem(null);
        }

        foreach (InventoryStorage storage in inventory)
        {
            if (storage.item != null)
            {
                storage.itemSlot.SetIfEmpty(false);

                storage.itemSlot.SetItem(storage.item);
                storage.itemSlot.SetQuantity(storage.quantity);
            }
        }
    }


    void DebugAddTestItems()
    {
        Debug.Log("Adding test items");

        AddToInventory(testItem, 100);
    }
}