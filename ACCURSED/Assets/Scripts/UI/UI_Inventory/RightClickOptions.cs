using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickOptions : MonoBehaviour
{
    public static RightClickOptions Instance { get; private set; }

    [SerializeField] public GameObject ui;
    [SerializeField] public GameObject back;

    [Header("Drop System")]
    [SerializeField] private RightClickDropController dropController;

    private bool isOpen;

    private void Awake()
    {
        isOpen = false;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dropController == null)
            dropController = GetComponent<RightClickDropController>();

        if (ui != null)
            ui.SetActive(false);

        if (back != null)
            back.SetActive(false);
    }

    public void Open(Inventory_ItemSlot slot)
    {
        if (slot == null)
            return;

        Vector3 uiPos = GetPosition(slot);

        if (ui != null && ui.TryGetComponent<RectTransform>(out RectTransform rectTransform))
            rectTransform.position = uiPos;

        if (dropController != null)
            dropController.OpenForSlot(slot);

        if (ui != null)
            ui.SetActive(true);

        isOpen = true;

        if (back != null)
            back.SetActive(true);
    }

    private Vector3 GetPosition(Inventory_ItemSlot slot)
    {
        Vector3 pos = slot.transform.position;
        pos.y -= 200f;
        pos.x += 400f;
        return pos;
    }

    public void Close()
    {
        if (dropController != null)
            dropController.OnMenuClosed();

        if (ui != null)
            ui.SetActive(false);

        isOpen = false;

        if (back != null)
            back.SetActive(false);
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    private void CheckLeftClick()
    {
        if (!GetIsOpen() || !Input.GetMouseButtonDown(0))
            return;

        if (PointerIsInsideOptions())
            return;

        Close();
    }

    private bool PointerIsInsideOptions()
    {
        if (ui == null || EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == null)
                continue;

            Transform hit = result.gameObject.transform;

            if (hit == ui.transform || hit.IsChildOf(ui.transform))
                return true;
        }

        return false;
    }

    private void Update()
    {
        CheckLeftClick();
    }
}
