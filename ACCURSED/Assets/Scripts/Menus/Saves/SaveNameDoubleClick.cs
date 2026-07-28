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

        if (saveSlot == null)
            return;

        // A single click only selects the slot.
        // Loading the save on the first click would make double-clicking impossible.
        saveSlot.SelectThisButton();

        if (eventData.clickCount >= 2)
            saveSlot.BeginRename();
    }
}