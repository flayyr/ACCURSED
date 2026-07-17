using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIDisplay : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI inputPromptText;
    [SerializeField] string inputBind;
    public void Instanciate(Sprite abilityIcon)
    {
        iconImage.sprite = abilityIcon;
        inputPromptText.text = inputBind;
    }
}
