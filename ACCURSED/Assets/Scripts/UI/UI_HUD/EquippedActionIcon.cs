using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedActionIcon : MonoBehaviour
{
    [Header("Equipment source")]
    [SerializeField] private StatusEquipmentCategory category = StatusEquipmentCategory.Items;
    [SerializeField, Min(0)] private int itemSlotIndex = 0;

    [Header("Display")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite emptyPlaceholder;

    [Header("Wake the existing CombatHUD on equipment changes")]
    [SerializeField] private CombatHUD combatHUD;
    [SerializeField] private bool wakeHUDWhenChanged = false;

    private StatusEquipmentManager itemManager;
    private StatusSkillEquipment skillManager;

    private void OnEnable()
    {
        CheckBindings();
        Refresh();
    }

    private void Update()
    {
        CheckBindings();
    }

    private void OnDisable()
    {
        if (itemManager != null)
            itemManager.EquipmentChanged -= OnEquipmentChanged;

        if (skillManager != null)
            skillManager.SkillsChanged -= OnEquipmentChanged;

        itemManager = null;
        skillManager = null;
    }

    private void CheckBindings()
    {
        StatusEquipmentManager newItemManager = StatusEquipmentManager.Instance;

        if (itemManager != newItemManager)
        {
            if (itemManager != null)
                itemManager.EquipmentChanged -= OnEquipmentChanged;

            itemManager = newItemManager;

            if (itemManager != null)
                itemManager.EquipmentChanged += OnEquipmentChanged;

            Refresh();
        }

        StatusSkillEquipment newSkillManager = null;
        if (PersistentPlayer.Instance != null)
            newSkillManager = PersistentPlayer.Instance.GetComponent<StatusSkillEquipment>();

        if (skillManager != newSkillManager)
        {
            if (skillManager != null)
                skillManager.SkillsChanged -= OnEquipmentChanged;

            skillManager = newSkillManager;

            if (skillManager != null)
                skillManager.SkillsChanged += OnEquipmentChanged;

            Refresh();
        }
    }

    private void OnEquipmentChanged()
    {
        Refresh();

        if (wakeHUDWhenChanged && combatHUD != null && combatHUD.isActiveAndEnabled)
            combatHUD.Show();
    }

    public void SetItemSlotIndex(int newIndex)
    {
        itemSlotIndex = Mathf.Max(0, newIndex);
        Refresh();
    }

    public void SetCategory(StatusEquipmentCategory newCategory)
    {
        category = newCategory;
        Refresh();
    }

    public void Refresh()
    {
        Sprite sprite = null;

        bool isInventoryItem = category == StatusEquipmentCategory.Items;

        if (skillManager != null)
        {
            AbilitySO skill = skillManager.GetEquippedSkill(category);

            if (skill != null)
                sprite = skill.abilityIcon;
        }

        if (iconImage != null)
        {
            iconImage.sprite = sprite != null ? sprite : emptyPlaceholder;
            iconImage.enabled = iconImage.sprite != null;
        }
    }
}
