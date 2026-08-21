using UnityEngine;

public class InventoryController : MonoBehaviour

{
    public static InventoryController Instance { get; private set; }

    [SerializeField] public GameObject ui;

    private bool isOpen;
    public static bool EscPressedThisFrame = false;

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

        ui.SetActive(true);

        isOpen = true;
    }

    public void CloseInventory() { 

        ui.SetActive(false);

        isOpen = false;
    }

    public bool getIsOpen()
    {
        return isOpen;
    }

}
