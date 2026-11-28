using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionKit : MonoBehaviour  //replaces PlayerAbilities
{
    [SerializeField] ActionSO baseAttackSO;
    [SerializeField] ActionSO dashActionSO;
    [SerializeField] HealingActionSO healingActionSO;
    [SerializeField] ActionSO parryActionSO;
    [SerializeField] VestigeSO vestigeAbilitySO;
    [SerializeField] RemembranceSO remembranceAbilitySO;

    [Header("New: placeholder Fable")]
    [SerializeField] private FableSO fableAbility;

    [HideInInspector] public event Action OnAbilityUsed;
    [HideInInspector] public event Action OnHealUsed;

    public event Action EquippedSkillsChanged;

    private AbilityUIDisplay vestigeUI;
    private AbilityUIDisplay remembranceUI;

    private float vestigeCDTimer;
    private readonly Dictionary<VestigeSO, float> vestigeTimers = new Dictionary<VestigeSO, float>();

    public VestigeSO EquippedVestige => vestigeAbilitySO;
    public RemembranceSO EquippedRemembrance => remembranceAbilitySO;
    public FableSO EquippedFable => fableAbility;

    ActionQueuer actionQueuer;
    PlayerData pData;

    ActionInstance currAttackAction;

    private void Awake() {
        actionQueuer = GetComponent<ActionQueuer>();
        pData = GetComponent<PlayerData>();
        if (pData != null && remembranceAbilitySO != null)
            pData.SetMaxRemembranceCharge(remembranceAbilitySO.requiredCharge);

        vestigeCDTimer = vestigeAbilitySO != null ? Mathf.Max(0f, vestigeAbilitySO.vestigeCoolDown) : 0f;

        if (vestigeAbilitySO != null)
            vestigeTimers[vestigeAbilitySO] = vestigeCDTimer;
    }

    private void OnEnable() {
        if (pData != null)
            pData.OnRemembranceChargeUpdate += UpdateRemembranceChargeUI;
    }

    private void OnDisable() {
        if (pData != null)
            pData.OnRemembranceChargeUpdate -= UpdateRemembranceChargeUI;
    }

    private void Update() {
        UpdateVestigeChargeUI();
    }

    public void CueAttack(bool isPressed) {
        if (isPressed) {
            currAttackAction = actionQueuer.QueueAction(baseAttackSO);
        } else {
            if (currAttackAction != null) {
                currAttackAction.skipWindWhenQueued = true;
            }
        }
    }

    public void CueDash() {
        if (pData.actionState is ActionState.StunnedCancellable) {
            pData.playerRef.playerActionManager.PlayActionInstantly(dashActionSO);
        } else {
            actionQueuer.QueueAction(dashActionSO);
        }
    }

    public bool CueRemembrance() {
        if (remembranceAbilitySO == null || pData == null || actionQueuer == null)
            return false;

        if (pData.currentRemembranceCharge < remembranceAbilitySO.requiredCharge)
            return false;

        actionQueuer.QueueAction(remembranceAbilitySO);
        pData.UpdateRemembranceCharge(-remembranceAbilitySO.requiredCharge);
        OnAbilityUsed?.Invoke();

        return true;
    }

    public bool CueVestige() {
        if (vestigeAbilitySO == null || actionQueuer == null)
            return false;

        if (vestigeCDTimer < vestigeAbilitySO.vestigeCoolDown)
            return false;

        actionQueuer.QueueAction(vestigeAbilitySO);
        vestigeCDTimer = 0f;
        vestigeTimers[vestigeAbilitySO] = 0f;
        vestigeUI?.SetFrameFill(vestigeAbilitySO.vestigeCoolDown <= 0f ? 1f : 0f);
        OnAbilityUsed?.Invoke();

        return true;
    }

    // Future entry point, not currently connected to a Player input action.
    public bool UseFable() {
        if (fableAbility == null || actionQueuer == null)
            return false;

        actionQueuer.QueueAction(fableAbility);
        OnAbilityUsed?.Invoke();

        return true;
    }

    public bool UseHeal() {
        if (pData != null && actionQueuer != null && healingActionSO != null && pData.CanHeal()) {
            actionQueuer.QueueAction(healingActionSO);
            OnHealUsed?.Invoke();
            return true;
        }
        return false;
    }

    public bool UseParry() {
        if (actionQueuer == null || parryActionSO == null)
            return false;

        actionQueuer.QueueAction(parryActionSO);
        return true;
    }

    public void InitializeUI(AbilityUIDisplay vestigeDisplay, AbilityUIDisplay remembranceDisplay) {
        vestigeUI = vestigeDisplay;
        remembranceUI = remembranceDisplay;
        RefreshAbilityUI();
    }

    /// <summary>
    /// Sets the actual actions used by UseRemembrance and UseVestige.
    /// Called by StatusSkillEquipment; null means the skill slot is unequipped.
    /// </summary>
    public void SetEquippedSkills(RemembranceSO remembrance, FableSO fable, VestigeSO vestige) {
        bool remembranceChanged = remembranceAbilitySO != remembrance;
        bool fableChanged = fableAbility != fable;
        bool vestigeChanged = vestigeAbilitySO != vestige;

        if (!remembranceChanged && !fableChanged && !vestigeChanged)
            return;

        remembranceAbilitySO = remembrance;
        fableAbility = fable;
        vestigeAbilitySO = vestige;

        if (remembranceChanged && pData != null)
            pData.SetMaxRemembranceCharge(remembranceAbilitySO != null ? remembranceAbilitySO.requiredCharge : 0f);

        if (vestigeChanged) {
            if (vestigeAbilitySO != null && !vestigeTimers.TryGetValue(vestigeAbilitySO, out vestigeCDTimer)) {
                vestigeCDTimer = Mathf.Max(0f, vestigeAbilitySO.vestigeCoolDown);
                vestigeTimers[vestigeAbilitySO] = vestigeCDTimer;
            }
        }

        RefreshAbilityUI();
        EquippedSkillsChanged?.Invoke();
    }

    private void RefreshAbilityUI() {
        if (vestigeUI != null) {
            vestigeUI.Initialize(vestigeAbilitySO != null ? vestigeAbilitySO.abilityIcon : null);
            float cooldown = vestigeAbilitySO != null ? vestigeAbilitySO.vestigeCoolDown : 0f;

            vestigeUI.SetFrameFill(cooldown <= 0f ? (vestigeAbilitySO != null ? 1f : 0f) : vestigeCDTimer / cooldown);
        }

        if (remembranceUI != null) {
            remembranceUI.Initialize(remembranceAbilitySO != null ? remembranceAbilitySO.abilityIcon : null);
            UpdateRemembranceChargeUI();
        }
    }

    private void UpdateVestigeChargeUI() {
        if (vestigeAbilitySO == null)
            return;

        float cooldown = vestigeAbilitySO.vestigeCoolDown;

        if (cooldown <= 0f) {
            vestigeUI?.SetFrameFill(1f);
            return;
        }

        if (vestigeCDTimer < cooldown) {
            vestigeCDTimer = Mathf.Min(vestigeCDTimer + Time.deltaTime, cooldown);
            vestigeTimers[vestigeAbilitySO] = vestigeCDTimer;
            vestigeUI?.SetFrameFill(vestigeCDTimer / cooldown);
        }
    }

    private void UpdateRemembranceChargeUI() {
        if (remembranceUI == null) return;

        float required = remembranceAbilitySO != null ? remembranceAbilitySO.requiredCharge : 0f;

        float fill = required <= 0f || pData == null ?
            (remembranceAbilitySO != null ? 1f : 0f) :
            Mathf.Clamp01(pData.currentRemembranceCharge / required);

        remembranceUI.SetFrameFill(fill);
    }
}
