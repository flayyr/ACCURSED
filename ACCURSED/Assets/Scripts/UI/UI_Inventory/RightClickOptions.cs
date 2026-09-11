using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class RightClickOptions : MonoBehaviour
{
    public static RightClickOptions Instance { get; private set; }

    [SerializeField] public GameObject ui;

    private bool isOpen;
    void Awake()
    {
        isOpen = false;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ui.SetActive(false);
    }

    void Open()
    {
        ui.SetActive(true);
        isOpen = true;
    }

    void Close()
    {
        ui.SetActive(false);
        isOpen = false;
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    void Update()
    {
        
    }
}
