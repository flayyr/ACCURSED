using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach to the clickable root Button of item and skill slots.
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
    private StatusSkillEquipment skills;
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
            yield return null;
            
        manager = StatusEquipmentManager.Instance;
        manager.EquipmentChanged += Refresh;

        if (category != StatusEquipmentCategory.Items)
        {
            while (skills == null) 
            { 
                skills = FindObjectOfType<StatusSkillEquipment>(); 
                
                if (skills == null) 
                    yield return null; 
            }

            skills.SkillsChanged += Refresh;
        }

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

        if (skills != null) 
            skills.SkillsChanged -= Refresh;

        skills = null;
        manager = null;
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(OnLeftClick);
    }

    private void OnLeftClick()
    {
        if (StatusEquipmentSelection.Instance != null)
            StatusEquipmentSelection.Instance.BeginSelection(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || manager == null)
            return;

        if (category == StatusEquipmentCategory.Items ? manager.GetEquippedItemID(category, slotIndex) == null : skills == null || skills.GetEquippedItem(category) == null)
            return;

        if (StatusUnequipPopup.Instance != null)
            StatusUnequipPopup.Instance.Open(this, eventData.position);
    }

    public void Unequip()
    {
        if (category == StatusEquipmentCategory.Items)
        { 
            if (manager != null) 
             manager.Unequip(category, slotIndex); 
        }
        else if (skills != null)
        { 
            string reason; 
            skills.TryEquipSkill(category, (SkillItemSO)null, out reason); 
        }
    }

    public void Refresh()
    {
        ItemPickupSO item = category == StatusEquipmentCategory.Items
            ? (manager != null ? manager.GetEquippedItem(category, slotIndex) : null)
            : (skills != null ? skills.GetEquippedItem(category) : null);

        int count = category == StatusEquipmentCategory.Items && manager != null
            ? manager.GetEquippedQuantity(category, slotIndex) : 0;

        if (itemIcon != null)
        {
            itemIcon.sprite = item != null ? item.itemSpr : null;
            itemIcon.enabled = item != null && item.itemSpr != null;
        }

        if (quantityText != null)
            quantityText.text = item != null && count > 0 && (count != 1 || showQuantityWhenOne) ? count.ToString() : "";
    }
}
