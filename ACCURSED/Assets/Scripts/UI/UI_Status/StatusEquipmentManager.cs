using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEquipmentCategory { Items, Soulbinds, Talismans, Remembrance, Fable, Vestige }

[Serializable]
public class StatusEquipmentEntry
{
    public StatusEquipmentCategory category = StatusEquipmentCategory.Items;

    [Min(0)] public int slotIndex;
    public string itemID;
}

public class StatusEquipmentManager : MonoBehaviour
{
    public static StatusEquipmentManager Instance { get; private set; }
    public event Action EquipmentChanged;

    [Header("Runtime equipment (NOT saved to disk yet)")]
    [SerializeField] private List<StatusEquipmentEntry> equipped = new List<StatusEquipmentEntry>();

    private PlayerInventory inventory;
    private Coroutine findInventoryRoutine;

    public PlayerInventory Inventory => inventory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (Instance != this) 
            return;
        
        if (inventory != null)
        {
            inventory.InventoryChanged -= OnInventoryChanged;
            inventory.InventoryChanged += OnInventoryChanged;
            OnInventoryChanged();
        }
        else findInventoryRoutine = StartCoroutine(FindInventory());
    }

    private IEnumerator FindInventory()
    {
        while (inventory == null)
        {
            PlayerInventory found = GetComponent<PlayerInventory>();
            if (found == null && PersistentPlayer.Instance != null)
                found = PersistentPlayer.Instance.GetComponent<PlayerInventory>();

            if (found != null)
            {
                BindInventory(found);
                findInventoryRoutine = null;

                yield break;
            }

            yield return null;
        }

        findInventoryRoutine = null;
    }

    private void BindInventory(PlayerInventory found)
    {
        if (inventory != null) 
            inventory.InventoryChanged -= OnInventoryChanged;

        inventory = found;
        inventory.InventoryChanged += OnInventoryChanged;
        OnInventoryChanged();
    }

    private void OnDisable()
    {
        if (findInventoryRoutine != null)
        {
            StopCoroutine(findInventoryRoutine);
            findInventoryRoutine = null;
        }

        if (inventory != null) 
            inventory.InventoryChanged -= OnInventoryChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnInventoryChanged()
    {
        if (inventory != null)
        {
            for (int i = equipped.Count - 1; i >= 0; --i)
            {
                StatusEquipmentEntry entry = equipped[i];

                if (entry == null || string.IsNullOrEmpty(entry.itemID) || inventory.GetQuantity(entry.itemID) <= 0)
                    equipped.RemoveAt(i);
            }
        }
        EquipmentChanged?.Invoke(); // Also refresh totals when quantity changes.
    }

    public string GetEquippedItemID(StatusEquipmentCategory category, int slotIndex)
    {
        foreach (StatusEquipmentEntry entry in equipped)
        {
            if (entry != null && entry.category == category && entry.slotIndex == slotIndex)
                return entry.itemID;
        }

        return null;
    }

    public ItemPickupSO GetEquippedItem(StatusEquipmentCategory category, int slotIndex)
    {
        string id = GetEquippedItemID(category, slotIndex);
        return inventory != null && !string.IsNullOrEmpty(id) ? inventory.GetItemData(id) : null;
    }

    public int GetEquippedQuantity(StatusEquipmentCategory category, int slotIndex)
    {
        string id = GetEquippedItemID(category, slotIndex);
        return inventory != null && !string.IsNullOrEmpty(id) ? inventory.GetQuantity(id) : 0;
    }

    public bool IsEquipped(string itemID)
    {
        if (string.IsNullOrEmpty(itemID)) 
            return false;
        
        foreach (StatusEquipmentEntry entry in equipped)
        {
            if (entry != null && entry.itemID == itemID)
                return true;
        }

        return false;
    }

    public bool TryEquip(StatusEquipmentCategory category, int slotIndex, ItemPickupSO item, out string reason)
    {
        reason = "";

        if (category != StatusEquipmentCategory.Items)
        {
            reason = "Only ITEMS slots are implemented in this version.";
            return false;
        }

        if (slotIndex < 0) 
        { 
            reason = "Invalid slot index."; 
            return false; 
        }

        if (inventory == null) 
        { 
            reason = "PlayerInventory has not been found."; 
            return false; 
        }

        if (item == null || !item.canUse || string.IsNullOrEmpty(item.itemID))
        {
            reason = "Only inventory items with canUse enabled can be equipped.";
            return false;
        }

        if (!inventory.HasItem(item.itemID))
        {
            reason = "You no longer have this item.";
            return false;
        }

        if (IsEquipped(item.itemID))
        {
            if (GetEquippedItemID(category, slotIndex) == item.itemID) 
                return true;

            reason = "That item type is already equipped in another slot.";
            return false;
        }

        for (int i = equipped.Count - 1; i >= 0; --i)
        {
            if (equipped[i] != null && equipped[i].category == category && equipped[i].slotIndex == slotIndex)
                equipped.RemoveAt(i);
        }

        equipped.Add(new StatusEquipmentEntry 
        {
            category = category, slotIndex = slotIndex, itemID = item.itemID
        });

        EquipmentChanged?.Invoke();
        return true;
    }

    public void Unequip(StatusEquipmentCategory category, int slotIndex)
    {
        bool removed = false;

        for (int i = equipped.Count - 1; i >= 0; --i)
        {
            StatusEquipmentEntry entry = equipped[i];
            if (entry != null && entry.category == category && entry.slotIndex == slotIndex)
            {
                equipped.RemoveAt(i);
                removed = true;
            }
        }

        if (removed) 
            EquipmentChanged?.Invoke();
    }
}
