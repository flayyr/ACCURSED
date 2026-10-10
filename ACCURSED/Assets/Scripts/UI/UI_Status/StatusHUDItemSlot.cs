using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusHUDItemSlot : MonoBehaviour
{
    [SerializeField] private StatusEquipmentCategory category = StatusEquipmentCategory.Items;
    [SerializeField, Min(0)] private int slotIndex;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private bool showQuantityWhenOne = true;

    private StatusEquipmentManager manager;
    private Coroutine bindRoutine;

    private void OnEnable()
    {
        bindRoutine = StartCoroutine(Bind());
    }

    private IEnumerator Bind()
    {
        while (StatusEquipmentManager.Instance == null) 
            yield return null;
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

    public void Refresh()
    {
        ItemPickupSO item = manager != null 
            ? manager.GetEquippedItem(category, slotIndex) : null;

        int count = manager != null 
            ? manager.GetEquippedQuantity(category, slotIndex) : 0;

        if (icon != null)
        {
            icon.sprite = item != null ? item.itemSpr : null;
            icon.enabled = item != null && item.itemSpr != null;
        }

        if (quantityText != null)
        {
            quantityText.text = item != null && count > 0 && (count != 1 || showQuantityWhenOne)
                ? count.ToString() : "";
        }
    }
}
