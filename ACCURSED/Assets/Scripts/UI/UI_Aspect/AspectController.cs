using UnityEngine;

public class AspectController : MonoBehaviour
{
    public static AspectController Instance { get; private set; }

    [SerializeField] private GameObject aspectMenu;

    private bool isOpen = false;

    void Awake()
    {
        aspectMenu.SetActive(false);

        // Singleton check (makes sure there is only one escape menu instance)
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
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        aspectMenu.SetActive(true);
        isOpen = true;
        //GamePauseController.Instance.PauseGame();
    }

    public void CloseMenu()
    {
        aspectMenu.SetActive(false);
        isOpen = false;
        //GamePauseController.Instance.ResumeGame();
    }

    public void OpenTravel()
    {

    }

    public bool getIsOpen()
    {
        return isOpen;
    }

}
