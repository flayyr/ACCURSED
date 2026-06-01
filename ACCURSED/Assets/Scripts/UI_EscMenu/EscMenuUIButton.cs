using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscMenuUIButton : MonoBehaviour
{   
    //ScriptableObject
    [SerializeField] private EscMenuUIButtonSO button;

    //Child objects
    [SerializeField] private GameObject imageObj;
    [SerializeField] private GameObject textObj;

    private Button uiButton;

    void Awake()
    {
        uiButton= GetComponent<Button>();

        // set image
        Image img = imageObj.GetComponent<Image>(); 
        img.sprite = button.buttonImage;

        // set text
        TextMeshProUGUI txt = textObj.GetComponent<TextMeshProUGUI>();
        txt.text = button.buttonText;

        uiButton.onClick.AddListener(OnButtonClicked);
        
    }

    private void OnButtonClicked()
    {
        button.Execute();
    }


}
