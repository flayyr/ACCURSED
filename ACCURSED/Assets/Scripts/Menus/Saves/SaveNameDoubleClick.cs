using UnityEngine;
using UnityEngine.EventSystems;

public class SaveNameDoubleClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SaveMenuButton saveSlot;

    private void Awake()
    {
        if (saveSlot == null)
            saveSlot = GetComponentInParent<SaveMenuButton>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (SaveNamePopup.IsOpen)
            return;

        if (saveSlot == null)
            return;

        // Clicking or double-clicking the name of an empty slot does nothing.
        if (!saveSlot.HasSave)
            return;

        // A single click on an existing save name only selects the slot. It does not load the save.
        saveSlot.SelectThisButton();

        // A double-click opens the rename popup.
        if (eventData.clickCount >= 2)
            saveSlot.BeginRename();
    }
}