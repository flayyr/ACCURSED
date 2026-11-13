using UnityEngine;
using UnityEngine.UI;

public class InventorySortByButton : MonoBehaviour
{
    public enum SortMode { Category, NewestPickup }

    [SerializeField] private SortMode mode = SortMode.Category;

    [SerializeField] private ItemCategory category;

    [SerializeField] private InventoryStorageManager storageManager;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (storageManager == null)
            storageManager = GetComponentInParent<InventoryStorageManager>(true);

        if (storageManager == null)
            storageManager = FindFirstObjectByType<InventoryStorageManager>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(SortByType);
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(SortByType);
    }

    private void SortByType()
    {
        if (storageManager == null)
        {
            Debug.LogError("InventorySortByButton: No InventoryStorageManager found.", this);
            return;
        }

        if (mode == SortMode.NewestPickup)
        {
            storageManager.SetSortByNewestPickup(!storageManager.IsSortedByNewestPickup);
            return;
        }

        if (storageManager.IsFilteringBy(category))
            storageManager.ClearCategoryFilter();
        else
            storageManager.SetCategoryFilter(category);
    }
}

