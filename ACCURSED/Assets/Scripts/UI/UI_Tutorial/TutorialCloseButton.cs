using UnityEngine;
using UnityEngine.UI;

public class TutorialCloseButton : MonoBehaviour
{

    public Button b;

    void Start()
    {
        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(ClosePopUp);
        Debug.Log("hello");
    }

    void ClosePopUp()
    {
        TutorialController.Instance.HideTutorial();
    }
}
