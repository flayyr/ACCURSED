using UnityEngine;
using System;

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

    [Header("World Identity")]
    [SerializeField] private string worldPickupID;

    private bool registeredWithItemManager;

    public string WorldPickupID => worldPickupID;
    public ItemPickupSO Item => item;

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

    private void Start()
    {
        TryRegisterWithItemManager();
    }

    private void Update()
    {
        if (!registeredWithItemManager)
            TryRegisterWithItemManager();

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

    private void TryRegisterWithItemManager()
    {
        if (registeredWithItemManager)
            return;

        if (ItemManager.Instance == null)
            return;


        ItemManager.Instance.RegisterWorldItem(this);

        registeredWithItemManager = true;
    }

    private void TryOpenPrompt()
    {
        if (ownsPrompt)
            return;

        if (ToolTipManager.Instance == null)
            return;

        if (activePickup != null && activePickup != this)
            return;

        if (ToolTipManager.Instance.IsPromptOpen)
            return;

        if (GlobalUIController.Instance != null && GlobalUIController.Instance.CheckIfOtherUIOpen())
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
        if (pickedUp)
            return;

        if (ItemManager.Instance == null)
        {
            Debug.LogError("Cannot pick up item because no ItemManager exists.", this);

            return;
        }

        ItemManager.Instance.CollectWorldItem(this);
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

    public void ApplyCollectedState()
    {
        pickedUp = true;
        ownsPrompt = false;


        if (activePickup == this)
            activePickup = null;
        
        gameObject.SetActive(false);
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
        if (!pickedUp && ownsPrompt)
        {
            if (ToolTipManager.Instance != null)
                ToolTipManager.Instance.ManuallyRemovePrompt();
        }

        if (activePickup == this)
            activePickup = null;
        
        ownsPrompt = false;

        if (registeredWithItemManager && ItemManager.Instance != null)
            ItemManager.Instance.UnregisterWorldItem(this);

        registeredWithItemManager = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }

    private void OnValidate()
    {
        // Don't assign an ID to the prefab asset itself.
        // Individual placed instances need their own IDs.
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
            return;

        if (!gameObject.scene.IsValid())
            return;
    }

#endif
}