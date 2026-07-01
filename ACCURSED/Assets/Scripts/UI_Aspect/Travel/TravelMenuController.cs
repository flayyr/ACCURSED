using UnityEngine;

public class TravelMenuController : MonoBehaviour
{
    public static TravelMenuController Instance { get; private set; }

    [SerializeField] private GameObject travelMenu;

    private bool isOpen = false;

    void Awake()
    {
        travelMenu.SetActive(false);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = false;
            TravelMenuController.Instance.CloseMenu();
        }
    }

    public void OpenMenu()
    {
        travelMenu.SetActive(true);
        isOpen = true;
        GamePauseController.Instance.PauseGame();
    }

    public void CloseMenu()
    {
        travelMenu.SetActive(false);
        isOpen = false;
        GamePauseController.Instance.ResumeGame();
    }

    public void OpenTravel()
    {

    }

}
