using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryStorageManager : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private GameObject itemContent;

    [SerializeField] private GameObject itemSlotPrefab;

    [SerializeField] private ItemInfoPanel infoPanel;

    private PlayerInventory playerInventory;

    private Coroutine bindRoutine;

    private bool hasFilter;

    private ItemCategory filterCategory;

    private bool sortByNewestPickup;

    public bool IsSortedByNewestPickup => sortByNewestPickup;

    public void SetSortByNewestPickup(bool enabled)
    {
        if (sortByNewestPickup == enabled)
            return;

        sortByNewestPickup = enabled;
        RebuildInventory();
    }

    public void SetCategoryFilter(ItemCategory category)
    {
        hasFilter = true;
        filterCategory = category;
        RebuildInventory();
    }

    public bool IsFilteringBy(ItemCategory category)
    {
        return hasFilter && filterCategory == category;
    }

    public void ClearCategoryFilter()
    {
        if (!hasFilter)
            return;

        hasFilter = false;
        RebuildInventory();
    }

    private void OnEnable()
    {
        bindRoutine = StartCoroutine(BindWhenPlayerExists());
    }

    private void OnDisable()
    {
        if (bindRoutine != null)
        {
            StopCoroutine(bindRoutine);
            bindRoutine = null;
        }

        if (playerInventory != null)
        {
            playerInventory.InventoryChanged -= RebuildInventory;
        }
    }

    private IEnumerator BindWhenPlayerExists()
    {
        while (playerInventory == null)
        {
            if (PersistentPlayer.Instance != null)
                playerInventory = PersistentPlayer.Instance.GetComponent<PlayerInventory>();

            // Fallback for scene testing.
            if (playerInventory == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                    playerInventory = player.GetComponent<PlayerInventory>();
            }

            if (playerInventory == null)
                yield return null;
        }

        playerInventory.InventoryChanged -= RebuildInventory;

        playerInventory.InventoryChanged += RebuildInventory;

        RebuildInventory();

        bindRoutine = null;
    }

    public void RebuildInventory()
    {
        if (playerInventory == null)
            return;

        if (itemContent == null)
        {
            Debug.LogError("InventoryStorageManager: " + "Item Content is not assigned.", this);

            return;
        }

        if (itemSlotPrefab == null)
        {
            Debug.LogError("InventoryStorageManager: " + "Item Slot Prefab is not assigned.", this);

            return;
        }

        Transform content = itemContent.transform;

        // Remove currently displayed slots.
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // Default: newest stacks first. Optionally: most recently picked up first.
        List<InventoryStack> ordered = new List<InventoryStack>();

        for (int i = playerInventory.Stacks.Count - 1; i >= 0; i--)
            ordered.Add(playerInventory.Stacks[i]);

        if (sortByNewestPickup)
            ordered = ordered.OrderByDescending(s => s != null ? s.pickupOrder : 0).ToList();

        foreach (InventoryStack stack in ordered)
        {
            if (stack == null || stack.item == null)
                continue;

            if (hasFilter && stack.item.itemCategory != filterCategory)
                continue;

            GameObject slotObject = Instantiate(itemSlotPrefab, content);

            Inventory_ItemSlot slot = slotObject.GetComponent<Inventory_ItemSlot>();

            if (slot == null)
            {
                Debug.LogError(
                    "Inventory slot prefab " +
                    "does not contain " +
                    "Inventory_ItemSlot.",
                    slotObject
                );

                continue;
            }

            slot.Bind(stack, playerInventory, infoPanel);
        }
    }
}