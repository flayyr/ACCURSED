using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Scene-level coordinator. 
public class StatusEquipmentSelection : MonoBehaviour
{
    public static StatusEquipmentSelection Instance { get; private set; }

    [SerializeField] private StatusPanelController statusPanel;
    [SerializeField] private InventoryController inventoryController;
    [Tooltip("Optional text on the inventory screen for errors and instructions")]
    [SerializeField] private TMP_Text feedbackText;
    
    [Header("Troubleshooting")]
    [Tooltip("Log why a click failed to select an inventory slot. Turn off after verifying setup.")]
    [SerializeField] private bool logClickDiagnostics = true;

    private StatusItemSlot target;
    private bool selecting;
    private int selectionStartedFrame;
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

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
        selectionStartedFrame = Time.frameCount;

        SetFeedback("Select a usable item to equip. Esc to cancel.");
        statusPanel.HideForEquipmentSelection();
        inventoryController.OpenInventory();

        if (!gameObject.activeInHierarchy || !enabled)
            Debug.LogError("StatusEquipmentSelection must live on an always-active UI manager, outside the hidden Status panel.", this);
    }

    public void OnInventorySlotClicked(Inventory_ItemSlot slot)
    {
        if (!selecting || target == null || slot == null) return;

        ItemPickupSO item = slot.BoundItem;

        if (item == null)
        {
            SetFeedback("This inventory slot is empty or not bound to an InventoryStack.");
            return;
        }

        StatusEquipmentManager manager = StatusEquipmentManager.Instance;

        if (manager == null)
        {
            SetFeedback("StatusEquipmentManager was not found. Attach it to the persistent Player.");
            return;
        }

        string reason;
        
        if (!manager.TryEquip(target.Category, target.SlotIndex, item, out reason))
        {
            SetFeedback(reason);
            return;
        }

        if (logClickDiagnostics)
            Debug.Log("Equipped '" + item.itemName + "' in ITEMS slot " + target.SlotIndex + ".", this);

        FinishSelection();
    }

    private void Update()
    {
        if (!selecting || Time.frameCount <= selectionStartedFrame)
            return;

        if (!Input.GetMouseButtonUp(0))
            return;

        if (EventSystem.current == null)
        {
            if (logClickDiagnostics)
                Debug.LogWarning("Equip click: no active EventSystem in scene.", this);

            return;
        }

        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count == 0)
        {
            if (logClickDiagnostics)
                Debug.LogWarning("Equip click: no UI raycast hit. Check Canvas GraphicRaycaster and Raycast Target on inventory slot graphics.", this);

            return;
        }

        GameObject top = raycastResults[0].gameObject;
        Inventory_ItemSlot clickedSlot = top != null
            ? top.GetComponentInParent<Inventory_ItemSlot>()
            : null;

        if (clickedSlot == null)
        {
            if (logClickDiagnostics)
                Debug.Log("Equip click hit '" + (top != null ? top.name : "null") + "', not an Inventory_ItemSlot. If this is a decorative overlay, disable its Image Raycast Target.", this);
            return;
        }

        if (inventoryController != null && inventoryController.ui != null && !clickedSlot.transform.IsChildOf(inventoryController.ui.transform))
        {
            if (logClickDiagnostics)
                Debug.LogWarning("Equip click hit a slot outside the InventoryController UI: " + clickedSlot.name, this);

            return;
        }

        OnInventorySlotClicked(clickedSlot);
    }

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
        if (selecting)
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
