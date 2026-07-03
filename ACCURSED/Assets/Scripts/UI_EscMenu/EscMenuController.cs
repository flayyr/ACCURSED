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
            /*if (!inventoryUI.activeSelf && !statusUI.activeSelf)
            {
                //Debug.Log("Toggle esc menu");
                ToggleEscMenu();
            }*/

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

        //GamePauseController.Instance.PauseGame();
    }

    public void CloseMenu()
    {
        isOpen = false;
        escMenu.SetActive(false);

        //GamePauseController.Instance.ResumeGame();
    }

    private void OnDestroy()
    {
        //GamePauseController.Instance.ResumeGame();
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