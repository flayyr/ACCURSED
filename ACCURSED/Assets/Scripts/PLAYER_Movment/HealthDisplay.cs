using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI vitalityText;
    [SerializeField] TextMeshProUGUI healChargeText;

    [SerializeField]PlayerStatistics stats;
    public void Initialize(PlayerStatistics characterStats)
    {
        stats = characterStats;
        UpdateHealthDisplay();
    }

    private void OnEnable()
    {
        stats.OnHealthUpdate += UpdateHealthDisplay;
        stats.OnVitalityUpdate += UpdateVitalityDisplay;
        stats.OnHealChargeUpdate += UpdateHealChargeDisplay;
    }

    private void OnDisable()
    {
        stats.OnHealthUpdate -= UpdateHealthDisplay;
        stats.OnVitalityUpdate -= UpdateVitalityDisplay;
        stats.OnHealChargeUpdate -= UpdateHealChargeDisplay;
    }

    private void UpdateHealthDisplay()
    {
        if (stats.currentHealth >= 2)
        {
            healthText.text = "Full health";
        }
        else if (stats.currentHealth >0)
        {
            healthText.text = "Damaged";
        }
        else
        {
            healthText.text = "Dead";
        }
    }

    private void UpdateVitalityDisplay()
    {
        vitalityText.text = stats.currentVitality+"";
    }

    private void UpdateHealChargeDisplay()
    {
        healChargeText.text = stats.currentHealCharge+"";
    }

}
