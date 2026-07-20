using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Image hpImage;
    [SerializeField] Sprite[] hpSprites;
    [SerializeField] Image vitalityImage;
    [SerializeField] TextMeshProUGUI healChargeText;

    [SerializeField]PlayerStatistics stats;
    public void Initialize(PlayerStatistics characterStats)
    {
        stats = characterStats;
        OnEnable();
    }

    private void OnEnable()
    {
        if (stats != null)
        {
            stats.OnHealthUpdate += UpdateHealthDisplay;
            stats.OnVitalityUpdate += UpdateVitalityDisplay;
            stats.OnHealChargeUpdate += UpdateHealChargeDisplay;
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
    }

    private void UpdateHealthDisplay()
    {
        if (stats.currentHealth >= 2)
        {
            hpImage.sprite = hpSprites[2];
        }
        else if (stats.currentHealth >0)
        {
            hpImage.sprite = hpSprites[1];
        }
        else
        {
            hpImage.sprite =hpSprites[0];
        }
    }

    private void UpdateVitalityDisplay()
    {
        vitalityImage.fillAmount = (float)stats.currentVitality / stats.maxVitality;
    }

    private void UpdateHealChargeDisplay()
    {
        healChargeText.text = stats.currentHealCharge+"";
    }

}
