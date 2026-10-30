using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerAbilities), typeof(PlayerInventory))]
public class StatusSkillEquipment : MonoBehaviour
{
    [Header("Permanent starting unlocks (grant once per Player instance)")]
    [SerializeField] private List<SkillItemSO> startingSkills = new List<SkillItemSO>();

    [Header("Starting equipped inventory skill assets")]
    [SerializeField] private SkillItemSO remembranceItem;
    [SerializeField] private SkillItemSO fableItem;
    [SerializeField] private SkillItemSO vestigeItem;

    public event Action SkillsChanged;
    public RemembranceSO EquippedRemembrance => remembranceItem != null ? remembranceItem.Ability as RemembranceSO : null;
    public FableSO EquippedFable => fableItem != null ? fableItem.Ability as FableSO : null;
    public VestigeSO EquippedVestige => vestigeItem != null ? vestigeItem.Ability as VestigeSO : null;

    private PlayerInventory inventory;
    private PlayerAbilities abilities;
    private bool grantedStartingSkills;
    private bool granting;
    private SkillItemSO lastRemembrance, lastFable, lastVestige;
    private bool appliedOnce;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        abilities = GetComponent<PlayerAbilities>();
    }

    private void OnEnable()
    {
        if (inventory != null) 
            inventory.InventoryChanged += SyncNow;
    }

    private void OnDisable()
    {
        if (inventory != null) 
            inventory.InventoryChanged -= SyncNow;
    }

    private void Start()
    {
        if (!grantedStartingSkills)
        {
            grantedStartingSkills = true;
            granting = true;

            Grant(remembranceItem);
            Grant(fableItem);
            Grant(vestigeItem);
            
            foreach (SkillItemSO skill in startingSkills)
                Grant(skill);

            granting = false;
        }

        SyncNow();
    }

    private void Grant(SkillItemSO skill)
    {
        if (skill != null && skill.IsValid && !inventory.HasItem(skill.itemID))
            inventory.AddItem(skill, 1);
    }

    private void Update() 
    { 
        SyncNow(); 
    }

    public SkillItemSO GetEquippedItem(StatusEquipmentCategory category)
    {
        switch (category)
        {
            case StatusEquipmentCategory.Remembrance: 
                return remembranceItem;

            case StatusEquipmentCategory.Fable: 
                return fableItem;

            case StatusEquipmentCategory.Vestige: 
                return vestigeItem;

            default: 
                return null;
        }
    }

    public AbilitySO GetEquippedSkill(StatusEquipmentCategory category)
    {
        SkillItemSO item = GetEquippedItem(category);
        
        return item != null ? item.Ability : null;
    }

    public bool IsEquipped(string itemID)
    {
        return !string.IsNullOrEmpty(itemID) && 
            ((remembranceItem != null && remembranceItem.itemID == itemID) || (fableItem != null && fableItem.itemID == itemID) || (vestigeItem != null && vestigeItem.itemID == itemID));
    }

    public bool TryEquipSkill(StatusEquipmentCategory category, SkillItemSO item, out string reason)
    {
        reason = "";
        if (category != StatusEquipmentCategory.Remembrance && category != StatusEquipmentCategory.Fable && category != StatusEquipmentCategory.Vestige)
        { 
            reason = "This is not a skill slot."; 
            return false; 
        }

        if (item != null && (!item.IsValid || item.SkillCategory != category || string.IsNullOrWhiteSpace(item.itemID) || inventory == null || !inventory.HasItem(item.itemID)))
        { 
            reason = "Select an owned skill matching this slot."; 
            return false; 
        }

        switch (category)
        {
            case StatusEquipmentCategory.Remembrance: 
                remembranceItem = item; 
                break;
            
            case StatusEquipmentCategory.Fable: 
                fableItem = item; 
                break;

            case StatusEquipmentCategory.Vestige: 
                vestigeItem = item; 
                 break;
        }

        SyncNow();
        return true;
    }

    public bool TryEquipSkill(StatusEquipmentCategory category, AbilitySO ability)
    {
        if (ability == null) 
        { 
            string ignored; 
            return TryEquipSkill(category, (SkillItemSO)null, out ignored); 
        }

        if (inventory == null) 
            return false;

        foreach (InventoryStack stack in inventory.Stacks)
        {
            SkillItemSO skill = stack != null ? stack.item as SkillItemSO : null;
            
            if (skill != null && skill.Ability == ability)
            { 
                string ignored; 
                return TryEquipSkill(category, skill, out ignored); 
            }
        }

        return false;
    }

    public bool SetSlotEnabled(StatusEquipmentCategory category, bool enabled)
    {
        if (enabled) 
            return GetEquippedItem(category) != null;

        string ignored;

        return TryEquipSkill(category, (SkillItemSO)null, out ignored);
    }

    public void SyncNow()
    {
        if (!Application.isPlaying || granting || abilities == null || inventory == null) 
            return;

        if (remembranceItem != null && (!remembranceItem.IsValid || remembranceItem.SkillCategory != StatusEquipmentCategory.Remembrance || !inventory.HasItem(remembranceItem.itemID))) 
            remembranceItem = null;

        if (fableItem != null && (!fableItem.IsValid || fableItem.SkillCategory != StatusEquipmentCategory.Fable || !inventory.HasItem(fableItem.itemID))) 
            fableItem = null;

        if (vestigeItem != null && (!vestigeItem.IsValid || vestigeItem.SkillCategory != StatusEquipmentCategory.Vestige || !inventory.HasItem(vestigeItem.itemID))) 
            vestigeItem = null;

        if (appliedOnce && lastRemembrance == remembranceItem && lastFable == fableItem && lastVestige == vestigeItem) 
            return;

        lastRemembrance = remembranceItem;
        lastFable = fableItem;
        lastVestige = vestigeItem;

        appliedOnce = true;
        abilities.SetEquippedSkills(EquippedRemembrance, EquippedFable, EquippedVestige);
        SkillsChanged?.Invoke();
    }
}
