using TMPro;
using UnityEngine;

// Scene-level coordinator that remains active while StatusPanel is hidden.
public class StatusEquipmentSelection : MonoBehaviour
{
    public static StatusEquipmentSelection Instance { get; private set; }

    [SerializeField] private StatusPanelController statusPanel;
    [SerializeField] private InventoryController inventoryController;
    [Tooltip("Optional TMP text on the inventory screen to explain invalid picks")]
    [SerializeField] private TMP_Text feedbackText;

    private StatusItemSlot target;
    private bool selecting;
    public bool IsSelecting => selecting;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
            return; 
}

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void BeginSelection(StatusItemSlot slot)
    {
        if (slot == null || slot.Category != StatusEquipmentCategory.Items) 
            return; 

        if (inventoryController == null)
            inventoryController = InventoryController.Instance;

        if (inventoryController == null || statusPanel == null)
        { 
            Debug.LogError("StatusEquipmentSelection: Assign StatusPanelController and InventoryController.", this);
            return;
        }

        if (RightClickOptions.Instance != null) 
            RightClickOptions.Instance.Close();
        
        if (StatusUnequipPopup.Instance != null) 
            StatusUnequipPopup.Instance.Close();

        target = slot;
        selecting = true;
        SetFeedback("Select a usable item to equip. Esc to cancel.");
        statusPanel.HideForEquipmentSelection();
        inventoryController.OpenInventory();
    }

    public void OnInventorySlotClicked(Inventory_ItemSlot slot)
    {
        if (!selecting || target == null || slot == null) 
            return;

        ItemPickupSO item = slot.BoundItem;

        if (item == null) 
            return;

        StatusEquipmentManager manager = StatusEquipmentManager.Instance;

        if (manager == null)
        {
            SetFeedback("Equipment manager was not found.");
            return;
        }

        string reason;

        if (!manager.TryEquip(target.Category, target.SlotIndex, item, out reason))
        {
            SetFeedback(reason);
            return;
        }

        FinishSelection();
    }

    // InventoryController already closes itself on Esc. Detect that and return to Status.
    private void LateUpdate()
    {
        if (!selecting) 
            return;

        if (inventoryController == null) 
            inventoryController = InventoryController.Instance;

        if (inventoryController != null && !inventoryController.getIsOpen())
            FinishSelection(false);
    }

    public void CancelSelection()
    {
        if (!selecting) 
            return;

        FinishSelection(true);
    }

    private void FinishSelection(bool closeInventory = true)
    {
        selecting = false;
        target = null;
        SetFeedback("");

        if (RightClickOptions.Instance != null) 
            RightClickOptions.Instance.Close();

        if (closeInventory && inventoryController != null && inventoryController.getIsOpen())
            inventoryController.CloseInventory();

        if (statusPanel != null) 
            statusPanel.ReturnFromEquipmentSelection();
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null) 
            feedbackText.text = message;
            
        if (!string.IsNullOrEmpty(message)) 
            Debug.Log("Equipment selection: " + message, this);
    }
}
