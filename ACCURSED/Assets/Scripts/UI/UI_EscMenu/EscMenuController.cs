using UnityEngine;
using System.Collections;

public class EscMenuController : MonoBehaviour
{
    public static EscMenuController Instance { get; private set; }

    [SerializeField] private GameObject escMenu;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject statusUI;

    [SerializeField] private CanvasGroup escMenuCanvas;
    private Coroutine menuAppear;

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

            if (!otherUIOpen())
            {
                ToggleEscMenu();
            }
        }
    }

    public void ToggleEscMenu()
    {
        if (isOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }

    }

    public void OpenMenu()
    {
        StopCurrentTransition();
        isOpen = true;
        escMenu.SetActive(true);
        menuAppear = StartCoroutine(MenuOpenRoutine());

        //GamePauseController.Instance.PauseGame();
        PersistentPlayer.controllerInstance.SetState(PlayerControlState.Disabled);
    }

    public void CloseMenu()
    {
        StopCurrentTransition();
        isOpen = false;
        menuAppear = StartCoroutine(MenuCloseRoutine());


        //GamePauseController.Instance.ResumeGame();
        PersistentPlayer.controllerInstance.SetState(PlayerControlState.Normal);
    }

    private IEnumerator MenuOpenRoutine()
    {
     
        escMenuCanvas.alpha = 0f;
        yield return UITransitions.Instance.FadeTransition(escMenuCanvas, 0f, 1f, 0.1f);
    }

    private IEnumerator MenuCloseRoutine()
    {
        escMenuCanvas.alpha = 1f;
        yield return UITransitions.Instance.FadeTransition(escMenuCanvas, 1f, 0f, 0.1f);
        escMenu.SetActive(false);
    }

    private void OnDestroy()
    {
        //
    }

    public void OpenStatus()
    {
        statusUI.SetActive(true);
    }

    public void OpenInventory()
    {
        InventoryController.Instance.OpenInventory();
    }

    private bool otherUIOpen()
    {
        if (
            // Aspect Menu
            AspectController.Instance != null && (AspectController.Instance.getIsOpen() || AspectController.EscPressedThisFrame)

            // Travel Menu
            || TravelMenuController.Instance != null && (TravelMenuController.Instance.getIsOpen() || TravelMenuController.EscPressedThisFrame)

            // Tutorial Popup
            || TutorialController.Instance != null && (TutorialController.Instance.getIsOpen() || TutorialController.EscPressedThisFrame)

            // Inventory
            || InventoryController.Instance != null && (InventoryController.Instance.getIsOpen() || InventoryController.EscPressedThisFrame)) 
        {
            return true;
        } 
        else 
        { 
            return false;  
        }
        
    }

    public bool getIsOpen()
    {
        return isOpen;
    }

    private void StopCurrentTransition()
    {
        if (menuAppear != null)
        {
            StopCoroutine(menuAppear);
            menuAppear = null;
        }
    }

}