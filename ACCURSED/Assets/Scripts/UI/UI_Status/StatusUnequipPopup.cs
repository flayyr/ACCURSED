using UnityEngine;
using UnityEngine.UI;

public class StatusUnequipPopup : MonoBehaviour
{
    public static StatusUnequipPopup Instance 
    { 
        get; 
        private set; 
    }

    [Header("Hierarchy")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private RectTransform menuRect;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button backdropButton;

    [Header("Position")]
    [Tooltip("Offset from the bottom-center of the equipped Status slot.")]
    [SerializeField] private Vector2 menuOffset = new Vector2(0f, -10f);

    private StatusItemSlot selectedSlot;

    public bool IsOpen => popupRoot != null && popupRoot.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (unequipButton != null)
            unequipButton.onClick.AddListener(OnUnequipClicked);

        if (backdropButton != null)
            backdropButton.onClick.AddListener(Close);

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        if (unequipButton != null)
            unequipButton.onClick.RemoveListener(OnUnequipClicked);

        if (backdropButton != null)
            backdropButton.onClick.RemoveListener(Close);

        if (Instance == this)
            Instance = null;
    }

    public void Open(StatusItemSlot slot, Vector2 screenPosition)
    {
        if (slot == null || popupRoot == null || menuRect == null)
            return;

        selectedSlot = slot;

        popupRoot.SetActive(true);
        popupRoot.transform.SetAsLastSibling();

        PositionBelowSlot(slot);
    }

    private void PositionBelowSlot(StatusItemSlot slot)
    {
        RectTransform slotRect = slot.GetComponent<RectTransform>();

        if (slotRect == null)
        {
            Debug.LogError("StatusUnequipPopup: StatusItemSlot " + "does not have a RectTransform.", slot);

            return;
        }

        menuRect.pivot = new Vector2(0.5f, 1f);

        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);

        // RectTransform corners:
        // 0 = bottom-left
        // 1 = top-left
        // 2 = top-right
        // 3 = bottom-right

        Vector3 bottomCenter = (corners[0] + corners[3]) * 0.5f;

        menuRect.position = bottomCenter;
        menuRect.anchoredPosition += menuOffset;
    }

    private void OnUnequipClicked()
    {
        if (selectedSlot != null)
            selectedSlot.Unequip();

        Close();
    }

    public void Close()
    {
        selectedSlot = null;

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }
}