using UnityEngine;
using System.Collections;

public class InventoryController : MonoBehaviour

{
    public static InventoryController Instance { get; private set; }

    [SerializeField] public GameObject ui;
    [SerializeField] public CanvasGroup inventoryCG;

    private bool isOpen;
    public static bool EscPressedThisFrame = false;
    private Coroutine menuAppear;

    void Awake()
    {
        isOpen = false;

        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ui.SetActive(false);
    }

    void Update()
    {

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            EscPressedThisFrame = true;
            isOpen = false;
            CloseInventory();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && EscPressedThisFrame)
        {
            EscPressedThisFrame = false;
        }
    }

    public void OpenInventory()
    {
        StopCurrentTransition();
        ui.SetActive(true);
        isOpen = true;

        menuAppear = StartCoroutine(InventoryOpenRoutine());
    }

    public void CloseInventory()
    {
        StopCurrentTransition();
        isOpen = false;

        menuAppear = StartCoroutine(InventoryCloseRoutine());
    }

    private IEnumerator InventoryOpenRoutine()
    {

        inventoryCG.alpha = 0f;
        yield return UITransitions.Instance.FadeTransition(inventoryCG, 0f, 1f, 0.1f);
    }

    private IEnumerator InventoryCloseRoutine()
    {
        inventoryCG.alpha = 1f;
        yield return UITransitions.Instance.FadeTransition(inventoryCG, 1f, 0f, 0.1f);
        ui.SetActive(false);
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
