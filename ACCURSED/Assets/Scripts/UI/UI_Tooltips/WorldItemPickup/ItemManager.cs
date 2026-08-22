using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Collected Items")]
    [SerializeField]
    private List<CollectedWorldItemRecord> collectedItems = new List<CollectedWorldItemRecord>();

    // All currently loaded/active world pickups.
    private Dictionary<string, WorldItemPickup> registeredWorldItems = new Dictionary<string, WorldItemPickup>();

    // Faster way of checking whether something was already collected.
    private HashSet<string> collectedWorldItemIds = new HashSet<string>();

    public IReadOnlyList<CollectedWorldItemRecord> CollectedItems => collectedItems;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        RebuildCollectedItemLookup();
    }

    // Called by WorldItemPickup when it becomes active.
    public void RegisterWorldItem(WorldItemPickup worldItem)
    {
        if (worldItem == null)
            return;

        string id = worldItem.WorldPickupID;

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("World item '" + worldItem.gameObject.name + "' has no World Pickup ID.", worldItem);

            return;
        }


        // Detect accidental duplicate IDs.
        if (registeredWorldItems.TryGetValue(id, out WorldItemPickup existing))
        {
            if (existing != null && existing != worldItem)
            {
                Debug.LogError(
                    $"Duplicate world pickup ID found!\n" +
                    $"{existing.gameObject.name}\n" +
                    $"{worldItem.gameObject.name}\n" +
                    $"ID: {id}",
                    worldItem
                );

                return;
            }
        }

        registeredWorldItems[id] = worldItem;

        // This object was collected during an earlier visit to this scene.
        if (HasBeenCollected(id))
            worldItem.ApplyCollectedState();
    }

    // Called when a world item is disabled or its scene unloads.
    public void UnregisterWorldItem(WorldItemPickup worldItem)
    {
        if (worldItem == null)
            return;

        string id = worldItem.WorldPickupID;

        if (string.IsNullOrEmpty(id))
            return;

        if (registeredWorldItems.TryGetValue(id, out WorldItemPickup registeredItem))
        {
            // Only remove it if this is actually the same object.
            if (registeredItem == worldItem)
                registeredWorldItems.Remove(id);
        }
    }


    // Called when the player successfully picks something up.
    public bool CollectWorldItem(WorldItemPickup worldItem)
    {
        if (worldItem == null)
            return false;

        string id = worldItem.WorldPickupID;

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Cannot collect '" + worldItem.gameObject.name + "' because it has no World Pickup ID.", worldItem);

            return false;
        }

        // Already taken.
        if (HasBeenCollected(id))
        {
            worldItem.ApplyCollectedState();
            return false;
        }
        
        ItemPickupSO item = worldItem.Item;

        if (item == null)
        {
            Debug.LogError("World pickup '" + worldItem.gameObject.name + "' does not have an ItemPickupSO.", worldItem);

            return false;
        }
        
        collectedWorldItemIds.Add(id);

        CollectedWorldItemRecord record = new CollectedWorldItemRecord
        {
            worldPickupId = id, 
            item = item,

            quantity = item.itemQuantity > 0
                ? item.itemQuantity
                : 1,

            sceneName = SceneManager.GetActiveScene().name
        };

        collectedItems.Add(record);

        Debug.Log("Collected: " + item.itemName + " + from " + worldItem.gameObject.name);

        // Record is already saved at this point.

        // First create the pickup indicator.
        ShowNormalItemPickup(item);

        // Then remove the world item.
        worldItem.ApplyCollectedState();

        return true;
    }

    public bool HasBeenCollected(string worldPickupId)
    {
        if (string.IsNullOrEmpty(worldPickupId))
            return false;

        return collectedWorldItemIds.Contains(worldPickupId);
    }

    public bool HasBeenCollected(WorldItemPickup worldItem)
    {
        if (worldItem == null)
            return false;

        return HasBeenCollected(worldItem.WorldPickupID);
    }

    private void RebuildCollectedItemLookup()
    {
        collectedWorldItemIds.Clear();

        foreach (CollectedWorldItemRecord record in collectedItems)
        {
            if (record == null)
                continue;

            if (string.IsNullOrEmpty(record.worldPickupId))
                continue;

            collectedWorldItemIds.Add(record.worldPickupId);
        }
    }

    private void ShowNormalItemPickup(ItemPickupSO item)
    {
        Debug.Log("ItemManager: attempting pickup UI for " + item.itemName);

        if (ToolTipManager.Instance == null)
        {
            Debug.LogError("ItemManager: ToolTipManager.Instance is NULL.");

            return;
        }

        NormalItemPickup normalPickup = ToolTipManager.Instance.GetComponent<NormalItemPickup>();

        if (normalPickup == null)
        {
            Debug.LogError("ItemManager: NormalItemPickup is NOT attached " + "to the ToolTipManager GameObject.");

            return;
        }

        Debug.Log("ItemManager: calling NormalItemPickup.AddItem().");

        normalPickup.AddItem(item);
    }
}

[Serializable]
public class CollectedWorldItemRecord
{
    public string worldPickupId;

    public ItemPickupSO item;

    public int quantity;

    public string sceneName;
}