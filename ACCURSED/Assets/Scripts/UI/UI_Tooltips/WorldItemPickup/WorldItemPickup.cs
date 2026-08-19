using UnityEngine;

public class WorldItemPickup : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private ItemPickupSO item;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool useItemSprite = true;

    [Header("Interaction")]
    [SerializeField] private float pickupDistance = 2f;
    [SerializeField] private string promptText = "Pick up";

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";

    private bool ownsPrompt;
    private bool pickedUp;

    // Prevent two nearby world pickups from both trying to control the pickup prompt at the same time.
    private static WorldItemPickup activePickup;


    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ApplyItemVisual();

        if (player == null)
            FindPlayer();
    }

    private void Update()
    {
        if (pickedUp)
            return;

        // Same as the camera
        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        float distanceSqr = (player.position - transform.position).sqrMagnitude;

        float pickupDistanceSqr = pickupDistance * pickupDistance;

        bool playerIsClose = distanceSqr <= pickupDistanceSqr;


        if (playerIsClose)
        {
            TryOpenPrompt();
        }
        else
        {
            ClosePrompt();
        }
    }

    private void TryOpenPrompt()
    {
        if (ownsPrompt)
            return;

        if (ToolTipManager.Instance == null)
            return;

        // Another world pickup currently owns the prompt.
        if (activePickup != null && activePickup != this)
            return;

        // A chest, NPC, etc. could already be using it.
        if (ToolTipManager.Instance.IsPromptOpen)
            return;

        if (GlobalUIController.Instance != null && GlobalUIController.Instance.CheckIfOtherUIOpen())
        {
            return;
        }

        activePickup = this;
        ownsPrompt = true;

        ToolTipManager.Instance.Prompt(GetPromptText(), item, PickUpItem);
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
        if (pickedUp)
            return;

        pickedUp = true;

        ownsPrompt = false;

        if (activePickup == this)
            activePickup = null;

        Debug.Log("Picked up: " + gameObject.name);

        gameObject.SetActive(false);
    }

    private string GetPromptText()
    {
        if (item == null)
            return promptText;

        if (string.IsNullOrEmpty(item.itemName))
            return promptText;

        return promptText + " " + item.itemName;
    }


    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            player = playerObject.transform;
    }


    private void ApplyItemVisual()
    {
        if (!useItemSprite)
            return;

        if (item == null)
            return;

        if (spriteRenderer == null)
            return;

        spriteRenderer.sprite = item.itemSpr;
    }


    private void OnDisable()
    {
        // If something disables the item while the player is looking at it, don't leave its prompt behind.
        if (!pickedUp && ownsPrompt)
        {
            if (ToolTipManager.Instance != null)
                ToolTipManager.Instance.ManuallyRemovePrompt();
        }

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