using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillItem", menuName = "Items/Skill Item")]
public class SkillItemSO : ItemPickupSO
{
    [SerializeField] private StatusEquipmentCategory skillCategory = StatusEquipmentCategory.Remembrance;
    [SerializeField] private AbilitySO ability;

    public StatusEquipmentCategory SkillCategory => skillCategory;
    public AbilitySO Ability => ability;

    public bool IsValid => ability != null &&
        ((skillCategory == StatusEquipmentCategory.Remembrance && ability is RemembranceSO) ||
         (skillCategory == StatusEquipmentCategory.Fable && ability is FableSO) ||
         (skillCategory == StatusEquipmentCategory.Vestige && ability is VestigeSO));

    private void OnValidate()
    {
        itemQuantity = 1;
        maxStackSize = 1;
        
        isSpecialItem = true;
        isImportantItem = true;
        canUse = false;

        if (ability != null && !IsValid)
            Debug.LogWarning("Skill Item ability must match its skill category.", this);
    }
}
