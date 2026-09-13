using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    private List<InventoryItemEntry> items = new List<InventoryItemEntry>();

    private Dictionary<string, InventoryItemEntry> itemLookup = new Dictionary<string, InventoryItemEntry>();

    public IReadOnlyList<InventoryItemEntry> Items => items;

    public event Action InventoryChanged;

    private void Awake()
    {
        RebuildLookup();
    }

    public bool AddItem(ItemPickupSO item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogError("PlayerInventory tried to add a null item.");

            return false;
        }
        
        if (string.IsNullOrWhiteSpace(item.itemID))
        {
            Debug.LogError(item.name + " does not have an itemID.", item);

            return false;
        }

        if (quantity <= 0)
        {
            quantity = 1;
        }

        // Already owns this item type.
        if (itemLookup.TryGetValue(item.itemID, out InventoryItemEntry existingEntry))
        {
            existingEntry.quantity += quantity;
        }
        else
        {
            InventoryItemEntry newEntry = new InventoryItemEntry
            {
                itemID = item.itemID,
                itemData = item,
                quantity = quantity
            };

            items.Add(newEntry);

            itemLookup.Add(item.itemID, newEntry);
        }

        Debug.Log(
            $"Inventory added: " +
            $"{item.itemName} x {quantity}. " +
            $"Total = {GetQuantity(item.itemID)}"
        );

        InventoryChanged?.Invoke();

        return true;
    }

    public bool HasItem(string itemID)
    {
        return itemLookup.ContainsKey(itemID);
    }

    public int GetQuantity(string itemID)
    {
        if (itemLookup.TryGetValue(itemID, out InventoryItemEntry entry))
            return entry.quantity;

        return 0;
    }

    public ItemPickupSO GetItemData(string itemID)
    {
        if (itemLookup.TryGetValue(itemID, out InventoryItemEntry entry))
            return entry.itemData;

        return null;
    }

    private void RebuildLookup()
    {
        itemLookup.Clear();

        foreach (InventoryItemEntry entry in items)
        {
            if (entry == null)
                continue;

            if (string.IsNullOrWhiteSpace(entry.itemID))
                continue;

            if (!itemLookup.ContainsKey(entry.itemID))
                itemLookup.Add(entry.itemID, entry);
        }
    }
}

[Serializable]
public class InventoryItemEntry
{
    public string itemID;

    public ItemPickupSO itemData;

    public int quantity;
}