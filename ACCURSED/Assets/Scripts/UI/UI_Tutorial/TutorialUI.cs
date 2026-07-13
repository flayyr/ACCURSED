using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{

    [SerializeField] public GameObject titleUI;
    [SerializeField] public GameObject instructionsUI;
    [SerializeField] public GameObject imageUI;

    public TutorialSO popup;

    private void Awake()
    {

    }

    public void Initialize(TutorialSO popUp)
    {

        // construct
        Debug.Log("L");
        popup = popUp;
        titleUI.GetComponent<TextMeshProUGUI>().text = popup.title;
        instructionsUI.GetComponent<TextMeshProUGUI>().text = popup.instructions;
        imageUI.GetComponent<Image>().sprite = popup.image;
    }



    private void Update()   
    {
        //Debug.Log("MouseDown:" + Input.GetMouseButtonDown(0));

    }
}
