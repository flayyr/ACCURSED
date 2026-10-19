using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerAbilities))]
public class StatusSkillEquipment : MonoBehaviour
{
    [Header("REMEMBRANCE")]
    [SerializeField] private bool remembranceEquipped = true;
    [SerializeField] private RemembranceSO remembrance;

    [Header("FABLE (placeholder)")]
    [SerializeField] private bool fableEquipped = false;
    [SerializeField] private FableSO fable;

    [Header("VESTIGE")]
    [SerializeField] private bool vestigeEquipped = true;
    [SerializeField] private VestigeSO vestige;

    public event Action SkillsChanged;

    public RemembranceSO EquippedRemembrance => remembranceEquipped ? remembrance : null;
    public FableSO EquippedFable => fableEquipped ? fable : null;
    public VestigeSO EquippedVestige => vestigeEquipped ? vestige : null;

    private PlayerAbilities playerAbilities;
    private RemembranceSO lastRemembrance;
    private FableSO lastFable;
    private VestigeSO lastVestige;
    private bool appliedOnce;

    private void Awake()
    {
        playerAbilities = GetComponent<PlayerAbilities>();
    }

    private void Start()
    {
        SyncNow();
    }

    private void Update()
    {
        SyncNow();
    }

    public AbilitySO GetEquippedSkill(StatusEquipmentCategory category)
    {
        switch (category)
        {
            case StatusEquipmentCategory.Remembrance: 
                return EquippedRemembrance;
                
            case StatusEquipmentCategory.Fable: 
                return EquippedFable;

            case StatusEquipmentCategory.Vestige: 
                return EquippedVestige;

            default: 
                return null;
        }
    }

    public bool TryEquipSkill(StatusEquipmentCategory category, AbilitySO newSkill)
    {
        switch (category)
        {
            case StatusEquipmentCategory.Remembrance:
                if (newSkill != null && !(newSkill is RemembranceSO)) 
                    return false;

                remembrance = (RemembranceSO)newSkill;
                remembranceEquipped = newSkill != null;
                break;

            case StatusEquipmentCategory.Fable:
                if (newSkill != null && !(newSkill is FableSO)) 
                    return false;

                fable = (FableSO)newSkill;
                fableEquipped = newSkill != null;
                break;

            case StatusEquipmentCategory.Vestige:
                if (newSkill != null && !(newSkill is VestigeSO)) 
                    return false;

                vestige = (VestigeSO)newSkill;
                vestigeEquipped = newSkill != null;
                break;

            default:
                return false;
        }

        SyncNow();
        return true;
    }

    public bool SetSlotEnabled(StatusEquipmentCategory category, bool isEnabled)
    {
        switch (category)
        {
            case StatusEquipmentCategory.Remembrance:
                remembranceEquipped = isEnabled;
                break;

            case StatusEquipmentCategory.Fable:
                fableEquipped = isEnabled;
                break;

            case StatusEquipmentCategory.Vestige:
                vestigeEquipped = isEnabled;
                break;

            default:
                return false;
        }

        SyncNow();
        return true;
    }

    public void SyncNow()
    {
        if (!Application.isPlaying) 
            return;

        if (playerAbilities == null)
            playerAbilities = GetComponent<PlayerAbilities>();

        if (playerAbilities == null) 
            return;

        RemembranceSO currentRemembrance = EquippedRemembrance;
        FableSO currentFable = EquippedFable;
        VestigeSO currentVestige = EquippedVestige;

        if (appliedOnce && lastRemembrance == currentRemembrance && lastFable == currentFable && lastVestige == currentVestige)
            return;

        playerAbilities.SetEquippedSkills(currentRemembrance, currentFable, currentVestige);

        lastRemembrance = currentRemembrance;
        lastFable = currentFable;
        lastVestige = currentVestige;
        appliedOnce = true;
        SkillsChanged?.Invoke();
    }
}
