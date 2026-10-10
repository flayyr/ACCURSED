using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StatusItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Identity (0 - 5, unique among ITEMS slots)")]
    [SerializeField] private StatusEquipmentCategory category = StatusEquipmentCategory.Items;
    [SerializeField, Min(0)] private int slotIndex;

    [Header("UI children")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private bool showQuantityWhenOne = true;

    private Button button;
    private StatusEquipmentManager manager;
    private Coroutine bindRoutine;

    public StatusEquipmentCategory Category => category;
    public int SlotIndex => slotIndex;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnLeftClick);
    }

    private void OnEnable()
    {
        bindRoutine = StartCoroutine(FindManager());
    }

    private IEnumerator FindManager()
    {
        while (StatusEquipmentManager.Instance == null) 
        {
            yield return null;
        }

        manager = StatusEquipmentManager.Instance;
        manager.EquipmentChanged += Refresh;

        Refresh();
        bindRoutine = null;
    }

    private void OnDisable()
    {
        if (bindRoutine != null) 
        {
            StopCoroutine(bindRoutine);
            bindRoutine = null;
        }

        if (manager != null) 
            manager.EquipmentChanged -= Refresh;
        
        manager = null;
    }

    private void OnDestroy()
    {
        if (button != null) 
            button.onClick.RemoveListener(OnLeftClick);
    }

    private void OnLeftClick()
    {
        if (category != StatusEquipmentCategory.Items) 
            return;

        if (StatusEquipmentSelection.Instance != null)
            StatusEquipmentSelection.Instance.BeginSelection(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || manager == null)
            return;

        if (manager.GetEquippedItemID(category, slotIndex) == null)
            return;

        if (StatusUnequipPopup.Instance != null)
            StatusUnequipPopup.Instance.Open(this, eventData.position);
    }

    public void Unequip()
    {
        if (manager != null) manager.Unequip(category, slotIndex);
    }

    public void Refresh()
    {
        ItemPickupSO item = manager != null 
            ? manager.GetEquippedItem(category, slotIndex) : null;

        int count = manager != null 
            ? manager.GetEquippedQuantity(category, slotIndex) : 0;

        if (itemIcon != null)
        {
            itemIcon.sprite = item != null ? item.itemSpr : null;
            itemIcon.enabled = item != null && item.itemSpr != null;
        }
        
        if (quantityText != null)
        {    
            quantityText.text = item != null && count > 0 && (count != 1 || showQuantityWhenOne)
                ? count.ToString() : "";
        }
    }
}
