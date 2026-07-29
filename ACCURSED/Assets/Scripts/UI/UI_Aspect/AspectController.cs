using UnityEngine;
using System.Collections;

public class AspectController : MonoBehaviour
{
    public static AspectController Instance { get; private set; }

    [SerializeField] private GameObject aspectMenu;
    [SerializeField] private CanvasGroup menuCanvas;

    private bool isOpen = false;
    public static bool EscPressedThisFrame = false;

    private Coroutine menuAppear;

    [HideInInspector]public AspectSO currentAspect;


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
        if (isOpen && Input.GetKeyDown(KeyCode.Escape) && !otherUIOpen())
        {
            EscPressedThisFrame = true;
            CloseMenu();
        }
        else if (!Input.GetKeyDown(KeyCode.Escape) && EscPressedThisFrame)
        {
            EscPressedThisFrame = false;
        }
    }

    public void OpenMenu(AspectSO openedAspect)
    {
        currentAspect = openedAspect;

        StopCurrentTransition();
        aspectMenu.SetActive(true);
        isOpen = true;

        menuAppear = StartCoroutine(MenuOpenRoutine());
    }

    public void CloseMenu()
    {
        StopCurrentTransition();
        isOpen = false;

        menuAppear = StartCoroutine(MenuCloseRoutine());
    }
    private IEnumerator MenuOpenRoutine()
    {

        menuCanvas.alpha = 0f;
        yield return UITransitions.Instance.FadeTransition(menuCanvas, 0f, 1f, 0.1f);
    }

    private IEnumerator MenuCloseRoutine()
    {
        menuCanvas.alpha = 1f;
        yield return UITransitions.Instance.FadeTransition(menuCanvas, 1f, 0f, 0.1f);
        aspectMenu.SetActive(false);
    }

    public void OpenTravel()
    {

    }

    public bool getIsOpen()
    {
        return isOpen;
    }

    private bool otherUIOpen()
    {
        if (TravelMenuController.Instance != null && (TravelMenuController.Instance.getIsOpen() || TravelMenuController.EscPressedThisFrame))
        //|| !inventoryUI.activeSelf && !statusUI.activeSelf;
        {
            return true;
        }
        else
        {
            return false;
        }
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
