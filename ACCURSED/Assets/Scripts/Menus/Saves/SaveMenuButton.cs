using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum ButtonType
    {
        SlotOne,
        SlotTwo,
        SlotThree,
        SlotFour
    }

    [Header("Button Type")]
    public ButtonType buttonType;

    [Header("Save Name")]
    [Tooltip("The text displayed above this save slot.")]
    [SerializeField] private TMP_Text saveNameText;

    [Tooltip("The popup used to enter or rename save names.")]
    [SerializeField] private SaveNamePopup saveNamePopup;

    [Header("Scene Loading")]
    [Tooltip("Scene loaded after creating or opening this save.")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Normal Sprites")]
    public Sprite slotOneNormalSprite;
    public Sprite slotTwoNormalSprite;
    public Sprite slotThreeNormalSprite;
    public Sprite slotFourNormalSprite;

    [Header("Selection Sprites")]
    public Sprite slotOneSelectedSprite;
    public Sprite slotTwoSelectedSprite;
    public Sprite slotThreeSelectedSprite;
    public Sprite slotFourSelectedSprite;

    private Image buttonImage;
    private bool isSelected;

    private static readonly List<SaveMenuButton> buttons = new List<SaveMenuButton>();

    private static int selectedIndex;

    public int SlotIndex
    {
        get { return (int)buttonType; }
    }

    public bool HasSave
    {
        get { return PlayerPrefs.GetInt(GetExistsKey(), 0) == 1; }
    }

    public string CurrentSaveName
    {
        get { return PlayerPrefs.GetString(GetNameKey(), "New Game"); }
    }

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (!buttons.Contains(this))
            buttons.Add(this);

        SortButtons();
        RefreshNameDisplay();
        SelectDefaultPlayButton();
    }

    private void OnDisable()
    {
        buttons.Remove(this);
    }

    private void Update()
    {
        // Only one save button handles the shared keyboard input.
        // This prevents every save button from reading the same key.
        if (buttons.Count == 0 || buttons[0] != this)
            return;

        // Do not navigate or activate save slots while the player is typing into the naming popup.
        if (SaveNamePopup.IsOpen)
            return;

        HandleKeyboardSelection();
        HandleConfirmInput();
    }

    private void HandleKeyboardSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            SelectButton(selectedIndex + 1);

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            SelectButton(selectedIndex - 1);
    }

    private void HandleConfirmInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedIndex >= 0 && selectedIndex < buttons.Count)
                buttons[selectedIndex].ActivateButton();
        }
    }

    public void ActivateButton()
    {
        if (SaveNamePopup.IsOpen)
            return;

        SelectThisButton();

        if (HasSave)
        {
            LoadExistingSave();
        }
        else
        {
            OpenNewSavePopup();
        }
    }

    private void OpenNewSavePopup()
    {
        if (saveNamePopup == null)
        {
            Debug.LogError("No SaveNamePopup has been assigned to " + gameObject.name);

            return;
        }

        // New empty slots always begin with "New Game" in the input field.
        // It is not permanently saved until Accept is pressed.
        saveNamePopup.OpenForNewSave(this);
    }

    public void BeginRename()
    {
        if (!HasSave)
            return;

        if (saveNamePopup == null)
        {
            Debug.LogError("No SaveNamePopup has been assigned to " + gameObject.name);

            return;
        }

        SelectThisButton();
        saveNamePopup.OpenForRename(this);
    }

    // Called by SaveNamePopup when Accept is pressed while making a new save.
    public void CreateNewSave(string enteredName)
    {
        string finalName = NormalizeSaveName(enteredName);

        PlayerPrefs.SetInt(GetExistsKey(), 1);
        PlayerPrefs.SetString(GetNameKey(), finalName);

        PlayerPrefs.Save();

        RefreshNameDisplay();

        Debug.Log("Created save slot " + (SlotIndex + 1) + " with the name: " + finalName);

        StartNewGame();
    }

    // Called by SaveNamePopup when Accept is pressed while renaming.
    public void RenameSave(string enteredName)
    {
        if (!HasSave)
            return;

        string finalName = NormalizeSaveName(enteredName);

        PlayerPrefs.SetString(GetNameKey(), finalName);
        PlayerPrefs.Save();

        RefreshNameDisplay();

        Debug.Log("Renamed save slot " + (SlotIndex + 1) + " to: " + finalName);
    }

    private string NormalizeSaveName(string enteredName)
    {
        if (string.IsNullOrWhiteSpace(enteredName))
            return "New Game";

        return enteredName.Trim();
    }

    private void StartNewGame()
    {
        // Your gameplay save manager can read these values after the game scene loads.
        PlayerPrefs.SetInt("ActiveSaveSlot", SlotIndex);
        PlayerPrefs.SetInt("ActiveSaveIsNew", 1);
        PlayerPrefs.Save();

        LoadGameScene();
    }

    private void LoadExistingSave()
    {
        PlayerPrefs.SetInt("ActiveSaveSlot", SlotIndex);
        PlayerPrefs.SetInt("ActiveSaveIsNew", 0);
        PlayerPrefs.Save();

        Debug.Log("Loading save slot " + (SlotIndex + 1) + ": " + CurrentSaveName);

        LoadGameScene();
    }

    private void LoadGameScene()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogWarning("No game scene has been assigned to " + gameObject.name);

            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void RefreshNameDisplay()
    {
        if (saveNameText == null)
            return;

        bool saveExists = HasSave;

        
        // An empty slot does not display a name.
        // Once the player accepts a new save name, the text becomes visible.
        
        saveNameText.gameObject.SetActive(saveExists);

        if (saveExists)
            saveNameText.text = CurrentSaveName;
    }

    public void SelectThisButton()
    {
        int index = buttons.IndexOf(this);

        if (index >= 0)
            SelectButton(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectThisButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // The slot remains selected after the pointer leaves.
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // If the player clicked the save-name text, let SaveNameDoubleClick handle it instead.
        GameObject clickedObject = eventData.pointerPressRaycast.gameObject;

        if (clickedObject != null && clickedObject.GetComponentInParent<SaveNameDoubleClick>() != null)
            return;

        ActivateButton();
    }

    private static void SelectButton(int index)
    {
        if (buttons.Count == 0)
            return;

        if (index < 0)
            index = buttons.Count - 1;

        if (index >= buttons.Count)
            index = 0;

        selectedIndex = index;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetSelected(i == selectedIndex);
        }
    }

    private void SetSelected(bool selected)
    {
        if (isSelected == selected)
            return;

        isSelected = selected;
        ChangeSprite();
    }

    private static void SelectDefaultPlayButton()
    {
        if (buttons.Count == 0)
            return;

        SaveMenuButton firstSlot = buttons.Find(button => button.buttonType == ButtonType.SlotOne);

        if (firstSlot != null)
        {
            SelectButton(buttons.IndexOf(firstSlot));
        }
        else
        {
            SelectButton(0);
        }
    }

    private void ChangeSprite()
    {
        if (buttonImage == null)
        {
            Debug.LogError("No Image component found on " + gameObject.name);

            return;
        }

        switch (buttonType)
        {
            case ButtonType.SlotOne:
                buttonImage.sprite = isSelected
                    ? slotOneSelectedSprite
                    : slotOneNormalSprite;
                break;

            case ButtonType.SlotTwo:
                buttonImage.sprite = isSelected
                    ? slotTwoSelectedSprite
                    : slotTwoNormalSprite;
                break;

            case ButtonType.SlotThree:
                buttonImage.sprite = isSelected
                    ? slotThreeSelectedSprite
                    : slotThreeNormalSprite;
                break;

            case ButtonType.SlotFour:
                buttonImage.sprite = isSelected
                    ? slotFourSelectedSprite
                    : slotFourNormalSprite;
                break;
        }
    }

    private string GetExistsKey()
    {
        return "SaveSlot_" + SlotIndex + "_Exists";
    }

    private string GetNameKey()
    {
        return "SaveSlot_" + SlotIndex + "_Name";
    }

    private static void SortButtons()
    {
        buttons.Sort((a, b) => a.transform.GetSiblingIndex()
                    .CompareTo(b.transform.GetSiblingIndex())
        );
    }
}