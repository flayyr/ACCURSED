using UnityEngine;
using UnityEngine.UI;

public class Aspect_Buttons : MonoBehaviour
{
    [SerializeField] private bool isTravel;
    [SerializeField] private bool isLeave;

    [SerializeField] public GameObject travelMenu;

    public Button b;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(ExecuteTask);
    }

    void ExecuteTask()
    {
        if (isTravel)
        {
            TravelMenuController.Instance.OpenMenu();
        }
        if (isLeave)
        {
            AspectController.Instance.CloseMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
