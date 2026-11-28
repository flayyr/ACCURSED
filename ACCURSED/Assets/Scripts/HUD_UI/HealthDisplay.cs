using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Image hpImage;
    [SerializeField] Sprite[] hpSprites;
    [SerializeField] Image vitalityImage;
    [SerializeField] Image vitalityFreezeImage;
    [SerializeField] TextMeshProUGUI healChargeText;

    PlayerStatistics stats;
    PlayerData pData;
    public void Initialize(PlayerStatistics characterStats)
    {
        stats = characterStats;
        OnEnable();
        UpdateHealthDisplay();
        UpdateVitalityDisplay();
        UpdateHealChargeDisplay();
    }
    public void Initialize(PlayerData data) {
        pData = data;
        OnEnable();
        UpdateHealthDisplay();
        UpdateVitalityDisplay();
        UpdateHealChargeDisplay();
    }

    private void OnEnable()
    {
        if (stats != null)
        {
            stats.OnHealthUpdate += UpdateHealthDisplay;
            stats.OnVitalityUpdate += UpdateVitalityDisplay;
            stats.OnHealChargeUpdate += UpdateHealChargeDisplay;
        }
        if (pData != null) {
            pData.OnHealthUpdate += UpdateHealthDisplay;
            pData.OnVitalityUpdate += UpdateVitalityDisplay;
            pData.OnHealChargeUpdate += UpdateHealChargeDisplay;
        }
    }

    private void OnDisable()
    {
        if (stats != null)
        {
            stats.OnHealthUpdate -= UpdateHealthDisplay;
            stats.OnVitalityUpdate -= UpdateVitalityDisplay;
            stats.OnHealChargeUpdate -= UpdateHealChargeDisplay;
        }
        if (pData != null) {
            pData.OnHealthUpdate -= UpdateHealthDisplay;
            pData.OnVitalityUpdate -= UpdateVitalityDisplay;
            pData.OnHealChargeUpdate -= UpdateHealChargeDisplay;
        }
    }

    private void UpdateHealthDisplay()
    {
        if (stats != null) {
            if (stats.GetHP() >= 2) {
                hpImage.sprite = hpSprites[2];
            } else if (stats.GetHP() > 0) {
                hpImage.sprite = hpSprites[1];
            } else {
                hpImage.sprite = hpSprites[0];
            }
        }

        if (pData != null) {
            if (pData.currHP >= 2) {
                hpImage.sprite = hpSprites[2];
            } else if (pData.currHP > 0) {
                hpImage.sprite = hpSprites[1];
            } else {
                hpImage.sprite = hpSprites[0];
            }
        }
    }

    private void UpdateVitalityDisplay()
    {
        if (stats != null) {
            vitalityImage.fillAmount = stats.currentVitality / stats.maxVitality;
            vitalityFreezeImage.fillAmount = stats.currentVitalityFreeze / stats.totalVitalityFreeze;
        }
        if (pData != null) {
            vitalityImage.fillAmount = pData.currentVitality / pData.maxVitality;
            vitalityFreezeImage.fillAmount = pData.currentVitalityFreeze / pData.totalVitalityFreeze;
        }
    }

    private void UpdateHealChargeDisplay()
    {
        if(stats != null)
            healChargeText.text = stats.currentHealCharge+"";

        if (pData != null)
            healChargeText.text = pData.currentHealCharge + "";
    }

}
