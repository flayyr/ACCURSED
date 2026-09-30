using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Collected Items")]
    [SerializeField] private List<CollectedWorldItemRecord> collectedItems = new List<CollectedWorldItemRecord>();

    // All currently loaded / active world pickups
    private Dictionary<string, WorldItemPickup> registeredWorldItems = new Dictionary<string, WorldItemPickup>();

    // Faster way of checking whether something was already collected
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

    private PlayerInventory FindPlayerInventory()
    {
        // Preferred method because your player is persistent.
        if (PersistentPlayer.Instance != null)
        {
            PlayerInventory inventory = PersistentPlayer.Instance.GetComponent<PlayerInventory>();

            if (inventory != null)
                return inventory;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
            return player.GetComponent<PlayerInventory>();

        return null;
    }

    // Called by WorldItemPickup when it becomes active.
    public void RegisterWorldItem(WorldItemPickup worldItem)
    {
        if (worldItem == null)
            return;

        string id = worldItem.WorldPickupID;

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("World item '" + worldItem.gameObject.name + "' has no World Pickup ID. ", worldItem);
            return;
        }

        // Detect accidental duplicate IDs
        if (registeredWorldItems.TryGetValue(id, out WorldItemPickup existing))
        {
            if (existing != null && existing != worldItem)
            {
                Debug.LogError(
                    "Duplicate world pickup ID found!" +
                    existing.gameObject.name +
                    worldItem.gameObject.name +
                    "ID: " + id,
                    worldItem);

                return;
            }
        }

        registeredWorldItems[id] = worldItem;

        // This object was collected during an earlier visit to this scene
        if (HasBeenCollected(id))
            worldItem.ApplyCollectedState();
    }

    // Called when a world item is disabled or its scene unloads
    public void UnregisterWorldItem(WorldItemPickup worldItem)
    {
        if (worldItem == null)
            return;

        string id = worldItem.WorldPickupID;

        if (string.IsNullOrEmpty(id))
            return;

        if (registeredWorldItems.TryGetValue(id, out WorldItemPickup registeredItem))
        {
            if (registeredItem == worldItem)
                registeredWorldItems.Remove(id);
        }
    }

    // Called when the player successfully picks something up
    public bool CollectWorldItem(WorldItemPickup worldItem)
    {
        if (worldItem == null)
            return false;

        ItemPickupSO item = worldItem.Item;

        if (item == null)
        {
            Debug.LogError(worldItem.name + " has no ItemPickupSO.", worldItem);

            return false;
        }

        string worldID = worldItem.WorldPickupID;

        if (string.IsNullOrWhiteSpace(worldID))
        {
            Debug.LogError(worldItem.name + " has no World Pickup ID.", worldItem);

            return false;
        }

        if (string.IsNullOrWhiteSpace(item.itemID))
        {
            Debug.LogError(item.name + " has no itemID.", item);
            return false;
        }

        if (HasBeenCollected(worldID))
        {
            worldItem.ApplyCollectedState();
            return false;
        }

        PlayerInventory inventory = FindPlayerInventory();

        if (inventory == null)
        {
            Debug.LogError("Cannot collect item because the Player does not have PlayerInventory.");
            return false;
        }

        int quantity = Mathf.Max(1, item.itemQuantity);

        if (!inventory.AddItem(item, quantity))
            return false;

        CollectedWorldItemRecord record = new CollectedWorldItemRecord
        {
            worldPickupID = worldID,
            item = item,
            quantity = quantity,
            sceneName = SceneManager.GetActiveScene().name
        };

        collectedItems.Add(record);
        collectedWorldItemIds.Add(worldID);

        worldItem.ApplyCollectedState();

        if (ToolTipManager.Instance != null)
            ToolTipManager.Instance.ShowNormalItemPickup(item);

        Debug.Log("Collected " + item.itemName + " x" + quantity + " | Item ID: " + item.itemID + " | World ID: " + worldID);

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

            if (string.IsNullOrEmpty(record.worldPickupID))
                continue;

            collectedWorldItemIds.Add(record.worldPickupID);
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
    public string worldPickupID;

    public ItemPickupSO item;

    public int quantity;

    public string sceneName;
}