using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inputPromptText;
    [SerializeField] string inputBind;
    [SerializeField] Image abilityImage;
    [SerializeField] Image abilityBackgroundImage;

    public void Initialize(Sprite abilityIcon)
    {
        if (abilityImage != null)
        {
            abilityImage.sprite = abilityIcon;
            abilityImage.enabled = abilityIcon != null;
        }

        if (abilityBackgroundImage != null)
        {
            abilityBackgroundImage.sprite = abilityIcon;
            abilityBackgroundImage.enabled = abilityIcon != null;
        }
        
        if (inputPromptText != null)
            inputPromptText.text = inputBind;
    }

    public void SetFrameFill(float fillAmt)
    {
        if (abilityImage != null)
            abilityImage.fillAmount = Mathf.Clamp01(fillAmt);
    }
}
