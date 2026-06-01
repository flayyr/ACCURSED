using UnityEngine;

public class EscMenuController : MonoBehaviour
{
    public static EscMenuController Instance { get; private set; }

    [SerializeField] private GameObject escMenu;
    private bool isOpen;
    private void Awake()
    {
        escMenu.SetActive(false);

        // Singleton check (makes sure there is only one escape menu instance)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

    Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscMenu();
        }
    }

    public void ToggleEscMenu()
    {
        isOpen = !isOpen;
        escMenu.SetActive(isOpen);
    }

    public void OpenMenu()
    {
        isOpen = true;
        escMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        isOpen = false;
        escMenu.SetActive(false);
    }

}