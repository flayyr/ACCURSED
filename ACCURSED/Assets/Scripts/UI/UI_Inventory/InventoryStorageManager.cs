using System.Collections;
using UnityEngine;

public class InventoryStorageManager : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField]
    private GameObject itemContent;

    [SerializeField]
    private GameObject itemSlotPrefab;

    [SerializeField]
    private ItemInfoPanel infoPanel;

    private PlayerInventory playerInventory;

    private Coroutine bindRoutine;

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
            playerInventory.InventoryChanged
                -= RebuildInventory;
        }
    }

    private IEnumerator BindWhenPlayerExists()
    {
        while (playerInventory == null)
        {
            // Preferred:
            // your persistent player.
            if (PersistentPlayer.Instance != null)
            {
                playerInventory =
                    PersistentPlayer.Instance
                        .GetComponent<PlayerInventory>();
            }

            // Fallback for scene testing.
            if (playerInventory == null)
            {
                GameObject player =
                    GameObject
                        .FindGameObjectWithTag(
                            "Player"
                        );

                if (player != null)
                {
                    playerInventory =
                        player.GetComponent<
                            PlayerInventory>();
                }
            }

            if (playerInventory == null)
                yield return null;
        }

        playerInventory.InventoryChanged
            -= RebuildInventory;

        playerInventory.InventoryChanged
            += RebuildInventory;

        RebuildInventory();

        bindRoutine = null;
    }

    public void RebuildInventory()
    {
        if (playerInventory == null)
            return;

        if (itemContent == null)
        {
            Debug.LogError(
                "InventoryStorageManager: " +
                "Item Content is not assigned.",
                this
            );

            return;
        }

        if (itemSlotPrefab == null)
        {
            Debug.LogError(
                "InventoryStorageManager: " +
                "Item Slot Prefab is not assigned.",
                this
            );

            return;
        }

        Transform content =
            itemContent.transform;

        // Remove currently displayed slots.
        for (int i = content.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                content.GetChild(i).gameObject
            );
        }

        // Newest stacks first.
        for (int i =
                 playerInventory.Stacks.Count - 1;
             i >= 0;
             i--)
        {
            InventoryStack stack =
                playerInventory.Stacks[i];

            if (stack == null ||
                stack.item == null)
            {
                continue;
            }

            GameObject slotObject =
                Instantiate(
                    itemSlotPrefab,
                    content
                );

            Inventory_ItemSlot slot =
                slotObject.GetComponent<
                    Inventory_ItemSlot>();

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

            slot.Bind(
                stack,
                playerInventory,
                infoPanel
            );
        }
    }
}