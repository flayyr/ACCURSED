using System;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private VestigeSO vestigeAbility;
    [SerializeField] private RemembranceSO remembranceAbility;
    [SerializeField] private ActionSO healAction;
    [SerializeField] private ActionSO parryAction;
    
    [Header("New: placeholder Fable")]
    [SerializeField] private FableSO fableAbility;

    [HideInInspector] public event Action OnAbilityUsed;
    [HideInInspector] public event Action OnHealUsed;
    public event Action EquippedSkillsChanged;

    private ActionQueuer actionQueuer;
    private PlayerStatistics playerStatistics;

    private AbilityUIDisplay vestigeUI;
    private AbilityUIDisplay remembranceUI;
    private float vestigeCDTimer;

    public VestigeSO EquippedVestige => vestigeAbility;
    public RemembranceSO EquippedRemembrance => remembranceAbility;
    public FableSO EquippedFable => fableAbility;

    private void Awake()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        playerStatistics = GetComponent<PlayerStatistics>();

        if (playerStatistics != null && remembranceAbility != null)
            playerStatistics.SetMaxRemembranceCharge(remembranceAbility.requiredCharge);

        vestigeCDTimer = vestigeAbility != null ? Mathf.Max(0f, vestigeAbility.vestigeCoolDown) : 0f;
    }

    private void OnEnable()
    {
        if (playerStatistics != null)
            playerStatistics.OnRemembranceChargeUpdate += UpdateRemembranceChargeUI;
    }

    private void OnDisable()
    {
        if (playerStatistics != null)
            playerStatistics.OnRemembranceChargeUpdate -= UpdateRemembranceChargeUI;
    }

    public void InitializeUI(AbilityUIDisplay vestigeDisplay, AbilityUIDisplay remembranceDisplay)
    {
        vestigeUI = vestigeDisplay;
        remembranceUI = remembranceDisplay;
        RefreshAbilityUI();
    }

    /// <summary>
    /// Sets the actual actions used by UseRemembrance and UseVestige.
    /// Called by StatusSkillEquipment; null means the skill slot is unequipped.
    /// </summary>
    public void SetEquippedSkills(RemembranceSO remembrance, FableSO fable, VestigeSO vestige)
    {
        bool remembranceChanged = remembranceAbility != remembrance;
        bool fableChanged = fableAbility != fable;
        bool vestigeChanged = vestigeAbility != vestige;
        if (!remembranceChanged && !fableChanged && !vestigeChanged)
            return;

        remembranceAbility = remembrance;
        fableAbility = fable;
        vestigeAbility = vestige;

        if (remembranceChanged && remembranceAbility != null && playerStatistics != null)
        {
            // The active Remembrance determines the maximum charge requirement.
            // Existing PlayerStatistics decides how current charge is handled.
            playerStatistics.SetMaxRemembranceCharge(remembranceAbility.requiredCharge);
        }

        if (vestigeChanged)
        {
            // A freshly equipped Vestige is ready; old cooldown does not carry over.
            vestigeCDTimer = vestigeAbility != null ? Mathf.Max(0f, vestigeAbility.vestigeCoolDown) : 0f;
        }

        RefreshAbilityUI();
        EquippedSkillsChanged?.Invoke();
    }

    private void RefreshAbilityUI()
    {
        if (vestigeUI != null)
        {
            vestigeUI.Initialize(vestigeAbility != null ? vestigeAbility.abilityIcon : null);
            float cooldown = vestigeAbility != null ? vestigeAbility.vestigeCoolDown : 0f;
            vestigeUI.SetFrameFill(cooldown <= 0f ? (vestigeAbility != null ? 1f : 0f) : vestigeCDTimer / cooldown);
        }

        if (remembranceUI != null)
        {
            remembranceUI.Initialize(remembranceAbility != null ? remembranceAbility.abilityIcon : null);
            UpdateRemembranceChargeUI();
        }
    }

    private void Update()
    {
        UpdateVestigeChargeUI();
    }

    private void UpdateVestigeChargeUI()
    {
        if (vestigeAbility == null) return;

        float cooldown = vestigeAbility.vestigeCoolDown;
        if (cooldown <= 0f)
        {
            vestigeUI?.SetFrameFill(1f);
            return;
        }

        if (vestigeCDTimer < cooldown)
        {
            vestigeCDTimer = Mathf.Min(vestigeCDTimer + Time.deltaTime, cooldown);
            vestigeUI?.SetFrameFill(vestigeCDTimer / cooldown);
        }
    }

    private void UpdateRemembranceChargeUI()
    {
        if (remembranceUI == null) return;

        float required = remembranceAbility != null ? remembranceAbility.requiredCharge : 0f;
        float fill = required <= 0f || playerStatistics == null
            ? (remembranceAbility != null ? 1f : 0f)
            : Mathf.Clamp01(playerStatistics.currentRemembranceCharge / required);

        remembranceUI.SetFrameFill(fill);
    }

    public bool UseRemembrance()
    {
        if (remembranceAbility == null || playerStatistics == null || actionQueuer == null)
            return false;

        if (playerStatistics.currentRemembranceCharge < remembranceAbility.requiredCharge)
            return false;

        actionQueuer.QueueAction(remembranceAbility);
        playerStatistics.UpdateRemembranceCharge(-remembranceAbility.requiredCharge);
        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseVestige()
    {
        if (vestigeAbility == null || actionQueuer == null) 
            return false;

        if (vestigeCDTimer < vestigeAbility.vestigeCoolDown) 
            return false;

        actionQueuer.QueueAction(vestigeAbility);
        vestigeCDTimer = 0f;
        vestigeUI?.SetFrameFill(vestigeAbility.vestigeCoolDown <= 0f ? 1f : 0f);
        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseFable()
    {
        if (fableAbility == null || actionQueuer == null) 
            return false;

        actionQueuer.QueueAction(fableAbility);
        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseHeal()
    {
        if (playerStatistics != null && actionQueuer != null && healAction != null && playerStatistics.CanHeal())
        {
            actionQueuer.QueueAction(healAction);
            OnHealUsed?.Invoke();
            return true;
        }
        return false;
    }

    public bool UseParry()
    {
        if (actionQueuer == null || parryAction == null) 
            return false;
            
        actionQueuer.QueueAction(parryAction);
        return true;
    }
}
