using UnityEngine;
using System.Collections;

public class TravelMenuController : MonoBehaviour
{
    public static TravelMenuController Instance { get; private set; }

    [SerializeField] private GameObject travelMenu;
    [SerializeField] private CanvasGroup menuCanvas;

    private bool isOpen = false;
    public static bool EscPressedThisFrame = false;

    private Coroutine menuAppear;

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
            EscPressedThisFrame = true;
            isOpen = false;
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && EscPressedThisFrame)
        {
            EscPressedThisFrame = false;
        }
    }

    public void OpenMenu()
    {
        StopCurrentTransition();
        travelMenu.SetActive(true);
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
        travelMenu.SetActive(false);
    }

    public void OpenTravel()
    {

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
