using System.Collections;
using UnityEngine;

// Put this on an always-active scene UI/controller GameObject, NOT the StatusPanel itself.
public class StatusPanelController : MonoBehaviour
{
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject escMenuRoot;

    [SerializeField] private bool openEscMenuWhenStatusCloses = true;
    [SerializeField] private bool hideStatusAtStart = true;

    public bool IsOpen => statusPanel != null && statusPanel.activeSelf;

    private void Start()
    {
        if (hideStatusAtStart && statusPanel != null)
            statusPanel.SetActive(false);
    }

    private void Update()
    {
        // InventoryController handles Esc in equipment selection mode.
        if (StatusEquipmentSelection.Instance != null &&
            StatusEquipmentSelection.Instance.IsSelecting) return;

        if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            // Close the right-click menu first, then the Status panel on a later Esc.
            if (StatusUnequipPopup.Instance != null && StatusUnequipPopup.Instance.IsOpen)
            {
                StatusUnequipPopup.Instance.Close();
            }
            else
            {
                CloseStatus();
            }
        }
    }

    public void OpenStatus()
    {
        if (StatusUnequipPopup.Instance != null) 
            StatusUnequipPopup.Instance.Close();

        if (escMenuRoot != null) 
            escMenuRoot.SetActive(false);

        if (statusPanel != null) 
            statusPanel.SetActive(true);
    }

    public void HideForEquipmentSelection()
    {
        if (statusPanel != null) 
            statusPanel.SetActive(false);
    }

    public void ReturnFromEquipmentSelection()
    {
        if (statusPanel != null) 
            statusPanel.SetActive(true);
    }

    public void CloseStatus()
    {
        if (StatusUnequipPopup.Instance != null) 
            StatusUnequipPopup.Instance.Close();

        if (statusPanel != null) 
            statusPanel.SetActive(false);

        if (openEscMenuWhenStatusCloses && escMenuRoot != null)
            StartCoroutine(ShowEscMenuNextFrame());
    }

    private IEnumerator ShowEscMenuNextFrame()
    {
        // Avoid delivering this same Esc key press to EscMenu's own Update method.
        yield return null;
        
        if (escMenuRoot != null) 
            escMenuRoot.SetActive(true);
    }
}
