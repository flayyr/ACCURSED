using UnityEngine;
using System.Collections.Generic;

public class NormalItemPickup : ItemPickup
{
    [Header("UI")]
    [SerializeField] private GameObject UINormalItemPickup;
    [SerializeField] private Transform container;

    // The queue now contains the actual UI instances, rather than storing runtime UI references in ItemPickupSO.
    public Queue<NormalItemPickupUI> itemPickupQueue = new Queue<NormalItemPickupUI>();

    private bool addedNewObjectThisUpdate = false;

    private PlayerController playerController;
    private bool subscribedToPlayer;

    private void Start()
    {
        TrySubscribeToPlayer();
    }

    private void Update()
    {
        // PersistentPlayer may not exist yet when Start runs.
        if (!subscribedToPlayer)
        {
            TrySubscribeToPlayer();
        }

        // Prevents the same Interact press used to pick up the item from immediately dismissing the newly-created UI.
        if (addedNewObjectThisUpdate)
        {
            addedNewObjectThisUpdate = false;
        }
    }

    private void TrySubscribeToPlayer()
    {
        if (subscribedToPlayer)
            return;

        if (PersistentPlayer.Instance == null)
            return;

        playerController = PersistentPlayer.Instance.GetComponent<PlayerController>();

        if (playerController == null)
            return;

        playerController.InteractKeyPressed += OnInteractPressed;

        subscribedToPlayer = true;
    }

    private void OnInteractPressed()
    {
        if (itemPickupQueue.Count == 0)
            return;

        // This was the same button press that CREATED the pickup popup. Don't immediately remove it.
        if (addedNewObjectThisUpdate)
            return;

        ConfirmItem();
    }

    public override void AddItem(ItemPickupSO item)
    {
        if (item == null)
        {
            Debug.LogError("NormalItemPickup received a null ItemPickupSO.");

            return;
        }

        if (UINormalItemPickup == null)
        {
            Debug.LogError("UI_NormalItemPickup prefab is not assigned.", this);

            return;
        }

        if (container == null)
        {
            Debug.LogError("NormalItemPickup container is not assigned.", this);

            return;
        }

        GameObject itemShowcase = Instantiate(UINormalItemPickup, container, false);


        itemShowcase.transform.SetAsFirstSibling();


        NormalItemPickupUI ui = itemShowcase.GetComponent<NormalItemPickupUI>();


        if (ui == null)
        {
            Debug.LogError("UI_NormalItemPickup is missing NormalItemPickupUI.", itemShowcase);

            Destroy(itemShowcase);

            return;
        }

        // Put it in the queue BEFORE initializing, so ManageStack can immediately find its position.
        itemPickupQueue.Enqueue(ui);


        ui.Initialize(item, this);


        addedNewObjectThisUpdate = true;


        // Creates the normal "OK" interaction prompt.
        ToolTipManager tooltip = GetComponent<ToolTipManager>();

        if (tooltip != null)
        {
            tooltip.Prompt("OK");
        }
    }

    public override void ConfirmItem()
    {
        if (itemPickupQueue.Count == 0)
            return;

        NormalItemPickupUI ui = itemPickupQueue.Dequeue();

        if (ui == null)
            return;

        ui.inQueue = false;

        ui.Confirmed();
    }

    public int GetQueuePosition(NormalItemPickupUI target)
    {
        int index = 0;

        foreach (NormalItemPickupUI ui in itemPickupQueue)
        {
            if (ui == target)
            {
                return index;
            }

            index++;
        }

        return -1;
    }


    private void OnDisable()
    {
        if (subscribedToPlayer && playerController != null)
        {
            playerController.InteractKeyPressed -= OnInteractPressed;
        }

        subscribedToPlayer = false;
        playerController = null;
    }
}