using UnityEngine;
using UnityEngine.UI;

// A small context menu using the SAME interaction pattern/styling as Use/Drop.
// Its visuals can be duplicated from the existing RightClickOptions prefab.
// This intentionally does NOT change the existing RightClickOptions implementation.
public class StatusUnequipPopup : MonoBehaviour
{
    public static StatusUnequipPopup Instance { get; private set; }

    [Header("Hierarchy: Root (fullscreen overlay) -> Backdrop/Button + Menu/Unequip Button")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private RectTransform menuRect;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button backdropButton;

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

        RectTransform parentRect = menuRect.parent as RectTransform;

        if (parentRect == null) 
            return;

        Canvas canvas = menuRect.GetComponentInParent<Canvas>();
        Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        Vector2 point;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPosition, eventCamera, out point))
            menuRect.anchoredPosition = point;
    }

    private void OnUnequipClicked()
    {
        if (selectedSlot != null) selectedSlot.Unequip();
        
        Close();
    }

    public void Close()
    {
        selectedSlot = null;

        if (popupRoot != null) popupRoot.SetActive(false);
    }
}
