using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveNamePopup : MonoBehaviour
{
    private enum PopupMode
    {
        CreateNewSave,
        RenameSave
    }

    [Header("Popup")]
    [SerializeField] private GameObject popupRoot;

    [Header("Input")]
    [SerializeField] private TMP_InputField saveNameInput;

    [Header("Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    public static bool IsOpen { get; private set; }

    private SaveMenuButton targetSlot;
    private PopupMode currentMode;

    // Prevents the Enter key that opened the popup from also accepting it during the same frame.
    private int openedFrame = -1;

    private void Awake()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(Accept);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Cancel);

        if (popupRoot != null)
            popupRoot.SetActive(false);

        IsOpen = false;
    }

    private void OnDestroy()
    {
        if (acceptButton != null)
            acceptButton.onClick.RemoveListener(Accept);

        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(Cancel);

        IsOpen = false;
    }

    private void Update()
    {
        if (!IsOpen)
            return;

        // Ignore input during the same frame in which the popup opened.
        if (Time.frameCount == openedFrame)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Accept();
    }

    public void OpenForNewSave(SaveMenuButton slot)
    {
        if (slot == null)
            return;

        targetSlot = slot;
        currentMode = PopupMode.CreateNewSave;

        // Every new save starts with this default name.
        OpenPopup("New Game");
    }

    public void OpenForRename(SaveMenuButton slot)
    {
        if (slot == null || !slot.HasSave)
            return;

        targetSlot = slot;
        currentMode = PopupMode.RenameSave;

        OpenPopup(slot.CurrentSaveName);
    }

    private void OpenPopup(string startingText)
    {
        if (popupRoot == null)
        {
            Debug.LogError("SaveNamePopup has no Popup Root assigned.");
            return;
        }

        if (saveNameInput == null)
        {
            Debug.LogError("SaveNamePopup has no input field assigned.");
            return;
        }

        popupRoot.transform.SetAsLastSibling();
        popupRoot.SetActive(true);

        IsOpen = true;
        openedFrame = Time.frameCount;

        saveNameInput.text = startingText;

        StartCoroutine(FocusInputNextFrame());
    }

    private IEnumerator FocusInputNextFrame()
    {
        yield return null;

        if (!IsOpen || saveNameInput == null)
            yield break;

        saveNameInput.Select();
        saveNameInput.ActivateInputField();

        // Select the existing name so typing immediately replaces it.
        // The player can also click inside the field to position the caret.
        saveNameInput.selectionAnchorPosition = 0;
        saveNameInput.selectionFocusPosition = saveNameInput.text.Length;
    }

    public void Accept()
    {
        if (!IsOpen || targetSlot == null)
            return;

        string enteredName = saveNameInput != null
            ? saveNameInput.text
            : "New Game";

        SaveMenuButton selectedSlot = targetSlot;
        PopupMode acceptedMode = currentMode;

        ClosePopup();

        if (acceptedMode == PopupMode.CreateNewSave)
        {
            selectedSlot.CreateNewSave(enteredName);
        }
        else
        {
            selectedSlot.RenameSave(enteredName);
        }
    }

    public void Cancel()
    {
        if (!IsOpen)
            return;

        ClosePopup();
    }

    private void ClosePopup()
    {
        IsOpen = false;
        openedFrame = -1;
        targetSlot = null;

        if (saveNameInput != null)
            saveNameInput.DeactivateInputField();

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }
}