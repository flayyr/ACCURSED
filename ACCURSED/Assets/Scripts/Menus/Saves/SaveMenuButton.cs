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

    [Header("Save Information")]
    [Tooltip("The save name displayed above this slot.")]
    [SerializeField] private TMP_Text saveNameText;

    [Tooltip("The popup used to create or rename saves.")]
    [SerializeField] private SaveNamePopup saveNamePopup;

    [Header("Delete Button")]
    [Tooltip("The Delete button belonging to this save slot.")]
    [SerializeField] private Button deleteButton;

    [Tooltip("Hide the Delete button when the slot is empty.")]
    [SerializeField] private bool hideDeleteButtonWhenEmpty = true;

    [Header("Scene Loading")]
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

    // Whether this slot actually contains a save.
    // The displayed name is not used to determine this because both an empty slot and a named save can be called "New Game."
    public bool HasSave
    {
        get
        {
            return PlayerPrefs.GetInt(GetExistsKey(), 0) == 1;
        }
    }

    public string CurrentSaveName
    {
        get
        {
            return PlayerPrefs.GetString(GetNameKey(), "New Game");
        }
    }

    private void Awake()
    {
        buttonImage = GetComponent<Image>();

        // Connect the Delete button automatically.
        // Do not also add DeleteSave manually to the button's On Click list.
        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeleteSave);
    }

    private void OnDestroy()
    {
        if (deleteButton != null)
            deleteButton.onClick.RemoveListener(DeleteSave);
    }

    private void OnEnable()
    {
        if (!buttons.Contains(this))
            buttons.Add(this);

        SortButtons();
        RefreshSlotDisplay();
        SelectDefaultPlayButton();
    }

    private void OnDisable()
    {
        buttons.Remove(this);
    }

    private void Update()
    {
        // Only the first registered button handles the shared keyboard navigation.
        if (buttons.Count == 0 || buttons[0] != this)
            return;

        // Do not navigate or activate slots while the player is entering a save name.
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
            Debug.LogError("No SaveNamePopup is assigned to " + gameObject.name);

            return;
        }

        saveNamePopup.OpenForNewSave(this);
    }

    public void BeginRename()
    {
        // Empty slots cannot be renamed because they do not yet contain a save.
        if (!HasSave)
        {
            return;
        }

        if (saveNamePopup == null)
        {
            Debug.LogError("No SaveNamePopup is assigned to " + gameObject.name);

            return;
        }

        SelectThisButton();
        saveNamePopup.OpenForRename(this);
    }

    public void CreateNewSave(string enteredName)
    {
        string finalName = NormalizeSaveName(enteredName);

        PlayerPrefs.SetInt(GetExistsKey(), 1);
        PlayerPrefs.SetString(GetNameKey(), finalName);
        PlayerPrefs.Save();

        RefreshSlotDisplay();

        Debug.Log("Created save slot " + (SlotIndex + 1) + " with the name: " + finalName);

        StartNewGame();
    }

    public void RenameSave(string enteredName)
    {
        if (!HasSave)
            return;

        string finalName = NormalizeSaveName(enteredName);

        PlayerPrefs.SetString(GetNameKey(), finalName);
        PlayerPrefs.Save();

        RefreshSlotDisplay();

        Debug.Log("Renamed save slot " + (SlotIndex + 1) + " to: " + finalName);
    }

    public void DeleteSave()
    {
        if (!HasSave)
        {
            RefreshSlotDisplay();
            return;
        }

        
        // Remove the values used by this naming system.
        PlayerPrefs.DeleteKey(GetExistsKey());
        PlayerPrefs.DeleteKey(GetNameKey());

        // If this slot was marked as the currently active slot, remove those values too.
        if (PlayerPrefs.GetInt("ActiveSaveSlot", -1) == SlotIndex)
        {
            PlayerPrefs.DeleteKey("ActiveSaveSlot");
            PlayerPrefs.DeleteKey("ActiveSaveIsNew");
        }

        // Add the deletion of your actual game-save data here once your full saving system is implemented.
        DeleteActualSlotData();

        PlayerPrefs.Save();

        RefreshSlotDisplay();

        Debug.Log("Deleted save slot " + (SlotIndex + 1) +". The slot is now empty.");
    }

    private void DeleteActualSlotData()
    {
        /*
        Placeholder for your future save system.
        
        Examples:
        SaveManager.DeleteSave(SlotIndex);
        
        or:
        
        string path = Application.persistentDataPath + "/SaveSlot_" + SlotIndex + ".json";
        
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
        
        */
    }

    private string NormalizeSaveName(string enteredName)
    {
        if (string.IsNullOrWhiteSpace(enteredName))
        {
            return "New Game";
        }

        return enteredName.Trim();
    }

    private void StartNewGame()
    {
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
            Debug.LogWarning("No game scene is assigned to " + gameObject.name);

            return;
        }

        RoomTransitionManager.Instance.BeginTransition(gameSceneName);
    }

    public void RefreshSlotDisplay()
    {
        /*
        The name text is now always visible.
        
        Empty slot:
            New Game
         
        Occupied slot:
            Its saved name
        */
        if (saveNameText != null)
        {
            saveNameText.gameObject.SetActive(true);

            if (HasSave)
            {
                saveNameText.text = CurrentSaveName;
            }
            else
            {
                saveNameText.text = "New Game";
            }
        }

        RefreshDeleteButton();
    }

    private void RefreshDeleteButton()
    {
        if (deleteButton == null)
            return;

        if (hideDeleteButtonWhenEmpty)
        {
            deleteButton.gameObject.SetActive(HasSave);
        }
        else
        {
            // Keep the button visible, but prevent it from being
            // pressed while the slot is empty.
            deleteButton.gameObject.SetActive(true);
            deleteButton.interactable = HasSave;
        }
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
        // Keep the slot selected after the pointer exits.
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        GameObject clickedObject = eventData.pointerPressRaycast.gameObject;

        //Do not activate/load the slot when the player is clicking its name or Delete button.
        if (clickedObject != null)
        {
            if (clickedObject.GetComponentInParent<SaveNameDoubleClick>() != null)
                return;

            if (deleteButton != null && clickedObject.transform.IsChildOf(deleteButton.transform))
                return;
        }

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
        {
            return;
        }

        isSelected = selected;
        ChangeSprite();
    }

    private static void SelectDefaultPlayButton()
    {
        if (buttons.Count == 0)
        {
            return;
        }

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
        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex())
        );
    }
}