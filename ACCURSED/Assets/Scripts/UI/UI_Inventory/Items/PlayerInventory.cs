using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryStack
{
    public ItemPickupSO item;
    public int quantity;

    public InventoryStack(ItemPickupSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Runtime Inventory")]
    [SerializeField]
    private List<InventoryStack> stacks = new List<InventoryStack>();

    public IReadOnlyList<InventoryStack> Stacks => stacks;

    public event Action InventoryChanged;

    /// <summary>
    /// Adds an amount of an item to the inventory.
    /// Existing stacks are filled first.
    /// New stacks are created when necessary.
    /// </summary>
    public bool AddItem(ItemPickupSO item, int quantity)
    {
        if (item == null)
        {
            Debug.LogError("Cannot add a null item to inventory.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(item.itemID))
        {
            Debug.LogError(item.name + " has no itemID.", item);
            return false;
        }

        if (quantity <= 0)
            return false;

        int remaining = quantity;
        int maxStack = Mathf.Max(1, item.maxStackSize);

        // Fill existing stacks first.
        foreach (InventoryStack stack in stacks)
        {
            if (stack == null || stack.item == null || stack.item.itemID != item.itemID)
                continue;

            int availableSpace = maxStack - stack.quantity;

            if (availableSpace <= 0)
                continue;

            int amountToAdd = Mathf.Min(availableSpace, remaining);

            stack.quantity += amountToAdd;
            remaining -= amountToAdd;

            if (remaining <= 0)
                break;
        }

        // Create new stacks if necessary.
        while (remaining > 0)
        {
            int stackAmount = Mathf.Min(maxStack, remaining);

            stacks.Add(new InventoryStack(item, stackAmount));

            remaining -= stackAmount;
        }

        InventoryChanged?.Invoke();

        return true;
    }

    public bool RemoveItem(string itemID, int quantity)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return false;

        if (quantity <= 0)
            return false;

        if (GetQuantity(itemID) < quantity)
            return false;

        int remaining = quantity;

        // Remove from newest stacks first.
        for (int i = stacks.Count - 1; i >= 0 && remaining > 0; i--)
        {
            InventoryStack stack = stacks[i];

            if (stack == null || stack.item == null || stack.item.itemID != itemID)
                continue;

            int amountToRemove = Mathf.Min(stack.quantity, remaining);

            stack.quantity -= amountToRemove;
            remaining -= amountToRemove;

            if (stack.quantity <= 0)
                stacks.RemoveAt(i);
        }

        InventoryChanged?.Invoke();

        return true;
    }

    public bool HasItem(string itemID)
    {
        return GetQuantity(itemID) > 0;
    }

    public int GetQuantity(string itemID)
    {
        int total = 0;

        foreach (InventoryStack stack in stacks)
        {
            if (stack == null || stack.item == null)
                continue;

            if (stack.item.itemID == itemID)
                total += stack.quantity;
        }

        return total;
    }

    public ItemPickupSO GetItemData(string itemID)
    {
        foreach (InventoryStack stack in stacks)
        {
            if (stack == null || stack.item == null)
                continue;

            if (stack.item.itemID == itemID)
                return stack.item;
        }

        return null;
    }
}