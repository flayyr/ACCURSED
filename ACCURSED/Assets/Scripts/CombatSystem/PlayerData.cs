using System;
using UnityEngine;

public class PlayerData : CharacterData
{
    public event Action OnHealChargeUpdate;
    public event Action OnVitalityUpdate;
    public event Action OnRemembranceChargeUpdate;

    public bool controlEnabled { get; set; } = true;

    [Header("Player Data")]
    [SerializeField] public PlayerReference playerRef;

    [SerializeField] private int maxHealCharge = 3;
    [SerializeField] public float maxVitality = 10;
    [SerializeField] public float freezeDrainPerSecond = 0.5f;

    private float maxRemembranceCharge;

    [HideInInspector] public int currentHealCharge;
    [HideInInspector] public float currentVitality;
    [HideInInspector] public float currentRemembranceCharge;

    [HideInInspector] public float currentVitalityFreeze;
    [HideInInspector] public float totalVitalityFreeze = 1;

    protected override void Start() {
        base.Start();

        OnHealChargeUpdate?.Invoke();
        OnVitalityUpdate?.Invoke();
        OnRemembranceChargeUpdate?.Invoke();  // combatUI might not have initialized the UI in player abilities yet
    }

    protected override void Update() {
        base.Update();

        if (currentVitalityFreeze > 0) {
            UpdateVitality(freezeDrainPerSecond * Time.deltaTime);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Minus)) {
            UpdateHealth(-1);
        }
#endif
    }

    public bool UseHealCharge() {
        if (CanHeal()) {
            UpdateHealth(1);
            currentHealCharge--;
            OnHealChargeUpdate?.Invoke();

            UpdateVitality(0);
            return true;
        }
        return false;
    }

    public bool CanHeal() {
        return currentHealCharge > 0 && currHP < maxHP;
    }

    public void SetVitalityFreeze(float freezeAmount) {
        totalVitalityFreeze = freezeAmount;
        currentVitalityFreeze = freezeAmount;
        UpdateVitality(0);
    }

    public float UpdateVitality(float vitalityChange) {
        float trueVitalityGain = vitalityChange;
        if (currentVitalityFreeze > 0) {
            currentVitalityFreeze -= vitalityChange;
            if (currentVitalityFreeze < 0) {
                trueVitalityGain = -currentVitalityFreeze;
                currentVitalityFreeze = 0;
            } else {
                trueVitalityGain = 0;
            }
        }

        currentVitality += trueVitalityGain;

        if (currentVitality >= maxVitality && currentHealCharge < maxHealCharge) {
            currentHealCharge++;
            currentVitality -= maxVitality;
            OnHealChargeUpdate?.Invoke();
        }
        currentVitality = Math.Min(currentVitality, maxVitality);
        OnVitalityUpdate?.Invoke();

        return currentVitality;
    }

    public float UpdateRemembranceCharge(float remembranceChargeChange) {
        currentRemembranceCharge += remembranceChargeChange;
        currentRemembranceCharge = Mathf.Clamp(currentRemembranceCharge, 0f, maxRemembranceCharge);
        OnRemembranceChargeUpdate?.Invoke();

        return currentRemembranceCharge;
    }

    public override void Reset() {
        base.Reset();

        currentHealCharge = maxHealCharge;
        currentVitality = 0;
        currentRemembranceCharge = maxRemembranceCharge;

        OnHealChargeUpdate?.Invoke();
        OnVitalityUpdate?.Invoke();
        OnRemembranceChargeUpdate?.Invoke();

        controlEnabled = true;
    }

    public void SetMaxRemembranceCharge(float maxRemembranceCharge) {
        this.maxRemembranceCharge = Mathf.Max(0f, maxRemembranceCharge);
        currentRemembranceCharge = Mathf.Clamp(currentRemembranceCharge, 0f, this.maxRemembranceCharge);
        OnRemembranceChargeUpdate?.Invoke();
    }

    protected override void ResolveBuff(BuffData buffData, bool isApplying) {
        base.ResolveBuff(buffData, isApplying);
        switch (buffData.type) {
            case BuffType.RimeflowerFreeze:
                if (isApplying)
                    playerRef.playerData.SetVitalityFreeze(buffData.amount);
                break;
            default:
                break;
        }
    }

}
