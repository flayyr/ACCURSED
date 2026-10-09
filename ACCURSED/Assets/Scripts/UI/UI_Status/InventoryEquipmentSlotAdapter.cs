using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach beside Inventory_ItemSlot on the RUNTIME inventory slot prefab.
[RequireComponent(typeof(Inventory_ItemSlot))]
[RequireComponent(typeof(Button))]
public class InventoryEquipmentSlotAdapter : MonoBehaviour, IPointerClickHandler
{
    [Header("Equipped visuals")]
    [SerializeField] private GameObject equippedBadge;
    [SerializeField] private Image itemImage;
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

        if (button != null)
            button.onClick.AddListener(HandleButtonClick);
    }

    private void OnEnable()
    {
        lastItemID = null;
        lastBlocked = false;
        lastEquipped = false;
    }

    private void Update()
    {
        if (slot == null) 
            return;

        ItemPickupSO item = slot.BoundItem;
        string itemID = item != null ? item.itemID : null;
        StatusEquipmentManager equipmentManager = StatusEquipmentManager.Instance;
        StatusEquipmentSelection selection = StatusEquipmentSelection.Instance;

        bool equipped = item != null && equipmentManager != null && equipmentManager.IsEquipped(itemID);
        bool selecting = selection != null && selection.IsSelecting;
        bool blockNormalRightClick = equipped || selecting;

        if (slot.enabled == blockNormalRightClick)
            slot.enabled = !blockNormalRightClick;

        if (lastEquipped != equipped || lastItemID != itemID || lastBlocked != blockNormalRightClick)
        {
            if (equippedBadge != null)
                equippedBadge.SetActive(equipped);

            if (itemImage != null && item != null)
                itemImage.color = equipped ? equippedTint : normalTint;

            lastEquipped = equipped;
            lastItemID = itemID;
            lastBlocked = blockNormalRightClick;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            TrySelectForEquipment();
    }

    private void HandleButtonClick()
    {
        TrySelectForEquipment();
    }

    private void TrySelectForEquipment()
    {
        StatusEquipmentSelection selection = StatusEquipmentSelection.Instance;
        if (selection == null || !selection.IsSelecting || slot == null)
            return;

        selection.OnInventorySlotClicked(slot);
    }

    private void OnDisable()
    {
        if (slot != null)
            slot.enabled = true;
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleButtonClick);
    }
}
