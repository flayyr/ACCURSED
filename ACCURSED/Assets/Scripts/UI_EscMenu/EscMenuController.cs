using UnityEngine;

public class EscMenuController : MonoBehaviour
{
    public static EscMenuController Instance { get; private set; }

    [SerializeField] private GameObject escMenu;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject statusUI;

    private bool isOpen = false;
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
            // if other menus are not open
            if (!inventoryUI.activeSelf && !statusUI.activeSelf)
            {
                ToggleEscMenu();
            }
        }
    }

    public void ToggleEscMenu()
    {
        isOpen = !isOpen;
        escMenu.SetActive(isOpen);

        //Time.timeScale = isOpen? 0f:1f;
    }

    public void OpenMenu()
    {
        isOpen = true;
        escMenu.SetActive(true);

        //Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        isOpen = false;
        escMenu.SetActive(false);

        //Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        //Time.timeScale = 1f;
    }

    public void OpenStatus()
    {
        statusUI.SetActive(true);
    }
    
    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
    }

}