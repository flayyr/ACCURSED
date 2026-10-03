using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightClickDropController : MonoBehaviour
{
    [Header("Panel States")]
    [SerializeField] private GameObject normalState;
    [SerializeField] private GameObject quantityState;
    [SerializeField] private GameObject confirmationState;

    [Header("Normal State Buttons")]
    [SerializeField] private Button dropOneButton;
    [SerializeField] private Button dropSelectedButton;

    [Header("Quantity State")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button quantityDropButton;
    [SerializeField] private Button quantityBackButton;

    [Header("Confirmation State")]
    [SerializeField] private TMP_Text confirmationText;
    [SerializeField] private Button confirmDropButton;
    [SerializeField] private Button confirmBackButton;

    [Header("Important Item World Drop")]
    [Tooltip("Use a duplicate of WorldItemPickup.prefab with WorldItemPickup removed and DroppedImportantItemPickup added.")]
    [SerializeField] private GameObject droppedImportantItemPrefab;

    [Tooltip("Optional. If empty, the player Transform is used.")]
    [SerializeField] private Transform dropOrigin;

    [SerializeField] private Vector3 dropOffset = new Vector3(0.75f, -0.25f, 0f);

    private PlayerInventory currentInventory;
    private ItemPickupSO currentItem;

    private int totalHeld;
    private int selectedQuantity = 1;
    private int pendingDropQuantity;

    private const string IrreversibleWarning =
        "Are you sure you want to drop this?\nThis cannot be reversed.";

    private void Awake()
    {
        if (dropOneButton != null)
            dropOneButton.onClick.AddListener(DropOnePressed);

        if (dropSelectedButton != null)
            dropSelectedButton.onClick.AddListener(DropSelectedPressed);

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreasePressed);

        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreasePressed);

        if (quantityDropButton != null)
            quantityDropButton.onClick.AddListener(QuantityDropPressed);

        if (quantityBackButton != null)
            quantityBackButton.onClick.AddListener(ResetToNormal);

        if (confirmDropButton != null)
            confirmDropButton.onClick.AddListener(ConfirmDropPressed);

        if (confirmBackButton != null)
            confirmBackButton.onClick.AddListener(ResetToNormal);

        if (confirmationText != null)
            confirmationText.text = IrreversibleWarning;

        ResetToNormal();
    }

    public void OpenForSlot(Inventory_ItemSlot slot)
    {
        if (slot == null)
        {
            ClearContext();
            return;
        }

        currentInventory = slot.BoundInventory;
        currentItem = slot.BoundItem;

        RefreshTotalHeld();

        selectedQuantity = 1;
        pendingDropQuantity = 0;

        ResetToNormal();
    }

    public void OnMenuClosed()
    {
        ResetToNormal();
        ClearContext();
    }

    public void ResetToNormal()
    {
        SetState(true, false, false);

        selectedQuantity = 1;
        pendingDropQuantity = 0;

        RefreshTotalHeld();
        UpdateQuantityText();
    }

    private void DropOnePressed()
    {
        RefreshTotalHeld();

        if (!CanDropAnything())
            return;

        BeginDrop(1);
    }

    private void DropSelectedPressed()
    {
        RefreshTotalHeld();

        if (!CanDropAnything())
            return;

        selectedQuantity = Mathf.Clamp(selectedQuantity, 1, totalHeld);
        UpdateQuantityText();

        SetState(false, true, false);
    }

    private void DecreasePressed()
    {
        RefreshTotalHeld();

        if (!CanDropAnything())
            return;

        selectedQuantity = Mathf.Max(1, selectedQuantity - 1);
        UpdateQuantityText();
    }

    private void IncreasePressed()
    {
        RefreshTotalHeld();

        if (!CanDropAnything())
            return;

        selectedQuantity = Mathf.Min(totalHeld, selectedQuantity + 1);
        UpdateQuantityText();
    }

    private void QuantityDropPressed()
    {
        RefreshTotalHeld();

        if (!CanDropAnything())
            return;

        selectedQuantity = Mathf.Clamp(selectedQuantity, 1, totalHeld);
        BeginDrop(selectedQuantity);
    }

    private void BeginDrop(int requestedQuantity)
    {
        RefreshTotalHeld();

        if (!CanDropAnything())
            return;

        int amount = Mathf.Clamp(requestedQuantity, 1, totalHeld);

        // Important items are recoverable, so the irreversible warning is skipped.
        if (currentItem.isImportantItem)
        {
            ExecuteDrop(amount);
            return;
        }

        pendingDropQuantity = amount;

        if (confirmationText != null)
            confirmationText.text = IrreversibleWarning;

        SetState(false, false, true);
    }

    private void ConfirmDropPressed()
    {
        RefreshTotalHeld();

        if (!CanDropAnything())
            return;

        int amount = Mathf.Clamp(pendingDropQuantity, 1, totalHeld);
        ExecuteDrop(amount);
    }

    private void ExecuteDrop(int amount)
    {
        if (currentInventory == null || currentItem == null)
            return;

        RefreshTotalHeld();

        if (totalHeld <= 0)
        {
            CloseMenu();
            return;
        }

        amount = Mathf.Clamp(amount, 1, totalHeld);

        if (currentItem.isImportantItem)
        {
            if (droppedImportantItemPrefab == null)
            {
                Debug.LogError(
                    "RightClickDropController: Dropped Important Item Prefab is not assigned.",
                    this
                );
                return;
            }

            if (droppedImportantItemPrefab.GetComponent<DroppedImportantItemPickup>() == null)
            {
                Debug.LogError(
                    "RightClickDropController: Dropped Important Item Prefab needs DroppedImportantItemPickup on its root.",
                    droppedImportantItemPrefab
                );
                return;
            }
        }

        if (!currentInventory.RemoveItem(currentItem.itemID, amount))
        {
            Debug.LogWarning(
                "RightClickDropController: PlayerInventory refused to remove " +
                currentItem.itemName + " x" + amount + ".",
                this
            );
            return;
        }

        if (currentItem.isImportantItem)
            SpawnImportantPickup(currentItem, amount);

        // Non-important items intentionally stop here: RemoveItem is the deletion.
        CloseMenu();
    }

    private void SpawnImportantPickup(ItemPickupSO item, int amount)
    {
        Transform playerTransform = FindPlayerTransform();

        Vector3 spawnPosition;

        if (dropOrigin != null)
            spawnPosition = dropOrigin.position;
        else if (playerTransform != null)
            spawnPosition = playerTransform.position + dropOffset;
        else
            spawnPosition = Vector3.zero;

        GameObject droppedObject = Instantiate(
            droppedImportantItemPrefab,
            spawnPosition,
            Quaternion.identity
        );

        DroppedImportantItemPickup pickup =
            droppedObject.GetComponent<DroppedImportantItemPickup>();

        pickup.Initialize(item, amount);
    }

    private Transform FindPlayerTransform()
    {
        if (PersistentPlayer.Instance != null)
            return PersistentPlayer.Instance.transform;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        return playerObject != null ? playerObject.transform : null;
    }

    private void RefreshTotalHeld()
    {
        if (currentInventory == null || currentItem == null ||
            string.IsNullOrWhiteSpace(currentItem.itemID))
        {
            totalHeld = 0;
            return;
        }

        totalHeld = currentInventory.GetQuantity(currentItem.itemID);

        if (totalHeld > 0)
            selectedQuantity = Mathf.Clamp(selectedQuantity, 1, totalHeld);
        else
            selectedQuantity = 1;
    }

    private bool CanDropAnything()
    {
        return currentInventory != null &&
               currentItem != null &&
               !string.IsNullOrWhiteSpace(currentItem.itemID) &&
               totalHeld > 0;
    }

    private void UpdateQuantityText()
    {
        if (quantityText != null)
            quantityText.text = selectedQuantity.ToString();
    }

    private void SetState(bool normal, bool quantity, bool confirmation)
    {
        if (normalState != null)
            normalState.SetActive(normal);

        if (quantityState != null)
            quantityState.SetActive(quantity);

        if (confirmationState != null)
            confirmationState.SetActive(confirmation);
    }

    private void CloseMenu()
    {
        if (RightClickOptions.Instance != null)
            RightClickOptions.Instance.Close();
        else
            OnMenuClosed();
    }

    private void ClearContext()
    {
        currentInventory = null;
        currentItem = null;
        totalHeld = 0;
        selectedQuantity = 1;
        pendingDropQuantity = 0;
    }
}
