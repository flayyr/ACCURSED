using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class RightClickOptions : MonoBehaviour
{
    public static RightClickOptions Instance { get; private set; }

    [SerializeField] public GameObject ui;
    [SerializeField] public GameObject back;

    private bool isOpen;
    //public bool escPressedThisFrame;
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
        back.SetActive(false);
    }

    public void Open(Inventory_ItemSlot slot)
    {
        //Debug.Log("Called");
        Vector3 uiPos = GetPosition(slot);
        if (ui.TryGetComponent<RectTransform>(out RectTransform rectTransform))
        {
            rectTransform.position = uiPos;
        }
        ui.SetActive(true);
        isOpen = true;
        back.SetActive(true);
    }

    private Vector3 GetPosition(Inventory_ItemSlot slot)
    {
        Vector3 pos = slot.GetComponent<Transform>().position;
        pos.y -= 200f;
        pos.x += 400f;
        return pos;
    }

    public void Close()
    {
        ui.SetActive(false);
        isOpen = false;
        back.SetActive(false);
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    void CheckLeftClick()
    {
        if (GetIsOpen() && Input.GetMouseButtonDown(0))
        {
            Close();
        }

    }


    void Update()
    {
        CheckLeftClick();
    }
}
