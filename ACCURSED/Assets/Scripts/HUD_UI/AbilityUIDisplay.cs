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
        abilityImage.sprite = abilityIcon;
        abilityBackgroundImage.sprite = abilityIcon;
        inputPromptText.text = inputBind;
    }

    public void SetFrameFill(float fillAmt)
    {
        abilityImage.fillAmount = fillAmt;
    }
}
