using System;
using UnityEngine;

public class PlayerStatistics : CharacterStatistics
{
    public event Action OnHealChargeUpdate;
    public event Action OnVitalityUpdate;
    public event Action OnRemembranceChargeUpdate;

    [SerializeField] private int maxHealCharge = 3;
    [SerializeField] public float maxVitality = 10;
    private float maxRemembranceCharge;

    [HideInInspector]public int currentHealCharge;
    [HideInInspector]public float currentVitality;
    [HideInInspector] public float currentRemembranceCharge;

    protected override void Start()
    {
        base.Start();

        OnHealChargeUpdate?.Invoke();
        OnVitalityUpdate?.Invoke();
        //OnRemembranceChargeUpdate?.Invoke();  commented because combatUI might not have initialized the UI in player abilities yet
    }

    public bool UseHealCharge()
    {
        if (CanHeal())
        {
            UpdateHealth(1);
            currentHealCharge--;
            OnHealChargeUpdate?.Invoke();

            UpdateVitality(0);
            return true;
        }
        return false;
    }

    public bool CanHeal()
    {
        return currentHealCharge > 0 && currentHealth < maxHealth;
    }

    public float UpdateVitality(float vitalityChange)
    {
        currentVitality += vitalityChange;
        if (currentVitality >= maxVitality && currentHealCharge<maxHealCharge)
        {
            currentHealCharge++;
            currentVitality -= maxVitality;
            OnHealChargeUpdate?.Invoke();
        }
        currentVitality = Math.Min(currentVitality, maxVitality);
        OnVitalityUpdate?.Invoke();
        return currentVitality;
    }

    public float UpdateRemembranceCharge(float remembranceChargeChange)
    {
        currentRemembranceCharge += remembranceChargeChange;
        currentRemembranceCharge = Math.Min(currentRemembranceCharge, maxRemembranceCharge);
        OnRemembranceChargeUpdate?.Invoke();

        return currentRemembranceCharge;
    }

    public override void Reset()
    {
        base.Reset();

        currentHealCharge = maxHealCharge;
        currentVitality = 0;
        currentRemembranceCharge = maxRemembranceCharge;

        OnHealChargeUpdate?.Invoke();
        OnVitalityUpdate?.Invoke();
        OnRemembranceChargeUpdate?.Invoke();
    }

    public void SetMaxRemembranceCharge(float maxRemembranceCharge)
    {
        this.maxRemembranceCharge = maxRemembranceCharge;
    }
}
