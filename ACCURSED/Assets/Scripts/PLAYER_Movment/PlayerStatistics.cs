using System;
using UnityEngine;

public class PlayerStatistics : CharacterStatistics
{
    public event Action OnHealChargeUpdate;
    public event Action OnVitalityUpdate;

    [SerializeField] private int maxHealCharge = 3;
    [SerializeField] public int maxVitality = 10;

    [HideInInspector]public int currentHealCharge;
    [HideInInspector]public int currentVitality;

    protected override void Start()
    {
        base.Start();
        currentHealCharge = maxHealCharge;
        currentVitality = 0;

        OnHealChargeUpdate?.Invoke();
        OnVitalityUpdate?.Invoke();
    }

    public bool UseHealCharge()
    {
        if (currentHealCharge > 0 && currentHealth<maxHealth)
        {
            UpdateHealth(1);
            currentHealCharge--;
            OnHealChargeUpdate?.Invoke();

            UpdateVitality(0);
            return true;
        }
        return false;
    }

    public int UpdateVitality(int vitalityChange)
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
}
