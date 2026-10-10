using UnityEngine;
using UnityEngine.UI;

// ADD TO THE EXISTING INVENTORY ITEM SLOT PREFAB (alongside Inventory_ItemSlot).
// Does not require changes to Inventory_ItemSlot or RightClickOptions.
[RequireComponent(typeof(Inventory_ItemSlot))]
[RequireComponent(typeof(Button))]
public class InventoryEquipmentSlotAdapter : MonoBehaviour
{
    [SerializeField] private GameObject equippedBadge;  // child icon with Raycast Target OFF
    [SerializeField] private Image itemImage;            // existing itemSprDisplay Image
    [SerializeField] private Color normalTint = Color.white;
    [SerializeField] private Color equippedTint = new Color(0.42f, 0.42f, 0.42f, 1f);

    private Inventory_ItemSlot slot;
    private Button button;
    private bool lastBlocked;
    private bool lastEquipped;
    private string lastItemID;

    private void Awake()
    {
        slot = GetComponent<Inventory_ItemSlot>();
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void OnEnable()
    {
        lastItemID = null;
        lastBlocked = false;
        lastEquipped = false;
    }

    private void Update()
    {
        ItemPickupSO item = slot != null ? slot.BoundItem : null;
        string itemID = item != null ? item.itemID : null;
        bool equipped = item != null && StatusEquipmentManager.Instance != null &&
                        StatusEquipmentManager.Instance.IsEquipped(itemID);
        bool selecting = StatusEquipmentSelection.Instance != null &&
                         StatusEquipmentSelection.Instance.IsSelecting;
        bool blockNormalRightClick = equipped || selecting;

        // This disables ONLY Inventory_ItemSlot's Update and pointer callbacks:
        // these are the two paths that call RightClickOptions.Open().
        // The Button's onClick listeners still work for normal left-click selection.
        if (slot != null && slot.enabled != !blockNormalRightClick)
            slot.enabled = !blockNormalRightClick;

        if (lastEquipped != equipped || lastItemID != itemID || lastBlocked != blockNormalRightClick)
        {
            if (equippedBadge != null) equippedBadge.SetActive(equipped);
            if (itemImage != null && item != null)
                itemImage.color = equipped ? equippedTint : normalTint;
            lastEquipped = equipped;
            lastItemID = itemID;
            lastBlocked = blockNormalRightClick;
        }
    }

    private void HandleClick()
    {
        if (StatusEquipmentSelection.Instance != null &&
            StatusEquipmentSelection.Instance.IsSelecting)
            StatusEquipmentSelection.Instance.OnInventorySlotClicked(slot);
    }

    private void OnDisable()
    {
        // Never leave the original script disabled when switching menus.
        if (slot != null) slot.enabled = true;
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(HandleClick);
    }
}
