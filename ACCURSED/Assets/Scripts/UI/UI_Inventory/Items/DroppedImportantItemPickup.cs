using UnityEngine;

public class DroppedImportantItemPickup : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool useItemSprite = true;

    [Header("Interaction")]
    [SerializeField] private float pickupDistance = 2f;
    [SerializeField] private string promptText = "Pick up";

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";

    private ItemPickupSO item;
    private int quantity = 1;

    private bool ownsPrompt;
    private bool pickedUp;

    private static DroppedImportantItemPickup activePickup;

    public ItemPickupSO Item => item;
    public int Quantity => quantity;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
            FindPlayer();
    }

    public void Initialize(ItemPickupSO newItem, int newQuantity)
    {
        item = newItem;
        quantity = Mathf.Max(1, newQuantity);

        ApplyItemVisual();

        if (player == null)
            FindPlayer();

        if (item == null)
            Debug.LogError("DroppedImportantItemPickup initialized with a null ItemPickupSO.", this);
    }

    private void Update()
    {
        if (pickedUp || item == null)
            return;

        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        float distanceSqr = (player.position - transform.position).sqrMagnitude;
        float pickupDistanceSqr = pickupDistance * pickupDistance;

        if (distanceSqr <= pickupDistanceSqr)
            TryOpenPrompt();
        else
            ClosePrompt();
    }

    private void TryOpenPrompt()
    {
        if (pickedUp || ownsPrompt)
            return;

        if (ToolTipManager.Instance == null)
            return;

        if (activePickup != null && activePickup != this)
            return;

        if (ToolTipManager.Instance.IsPromptOpen)
            return;

        if (GlobalUIController.Instance != null &&
            GlobalUIController.Instance.CheckIfOtherUIOpen())
            return;

        activePickup = this;
        ownsPrompt = true;

        ToolTipManager.Instance.Prompt(GetPromptText(), PickUpItem);
    }

    private void ClosePrompt()
    {
        if (!ownsPrompt)
            return;

        if (ToolTipManager.Instance != null)
            ToolTipManager.Instance.ManuallyRemovePrompt();

        ownsPrompt = false;

        if (activePickup == this)
            activePickup = null;
    }

    private void PickUpItem()
    {
        if (pickedUp || item == null)
            return;

        PlayerInventory inventory = FindPlayerInventory();

        if (inventory == null)
        {
            Debug.LogError(
                "DroppedImportantItemPickup: Player does not have PlayerInventory.",
                this
            );
            return;
        }

        if (!inventory.AddItem(item, quantity))
            return;

        pickedUp = true;
        ClosePrompt();

        if (ToolTipManager.Instance != null)
            ToolTipManager.Instance.ShowNormalItemPickup(item);

        Destroy(gameObject);
    }

    private PlayerInventory FindPlayerInventory()
    {
        if (PersistentPlayer.Instance != null)
        {
            PlayerInventory persistentInventory =
                PersistentPlayer.Instance.GetComponent<PlayerInventory>();

            if (persistentInventory != null)
                return persistentInventory;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            return playerObject.GetComponent<PlayerInventory>();

        return null;
    }

    private string GetPromptText()
    {
        if (item == null || string.IsNullOrEmpty(item.itemName))
            return promptText;

        if (quantity > 1)
            return promptText + " " + item.itemName + " x" + quantity;

        return promptText + " " + item.itemName;
    }

    private void FindPlayer()
    {
        if (PersistentPlayer.Instance != null)
        {
            player = PersistentPlayer.Instance.transform;
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void ApplyItemVisual()
    {
        if (!useItemSprite || item == null || spriteRenderer == null)
            return;

        spriteRenderer.sprite = item.itemSpr;
    }

    private void OnDisable()
    {
        if (!pickedUp && ownsPrompt && ToolTipManager.Instance != null)
            ToolTipManager.Instance.ManuallyRemovePrompt();

        if (activePickup == this)
            activePickup = null;

        ownsPrompt = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
#endif
}
