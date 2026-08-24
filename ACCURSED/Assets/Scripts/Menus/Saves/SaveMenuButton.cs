using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    
    [Header("Slot References")]
    [Tooltip("Dark rectangle behind the slot frame.")]
    [SerializeField] private Image backgroundImage;

    [Tooltip("The frame image displayed above the background.")]
    [SerializeField] private Image frameImage;

    [Tooltip("The save name displayed above the slot.")]
    [SerializeField] private TMP_Text saveNameText;

    [Tooltip("Delete button belonging to this slot.")]
    [SerializeField] private Button deleteButton;

    [Tooltip("Popup used to create and rename saves.")]
    [SerializeField] private SaveNamePopup saveNamePopup;
    
    [Header("Selection - Scale")]
    [Tooltip("How much larger the entire slot becomes when selected.")]
    [SerializeField] private float selectedScale = 1.08f;

    [Tooltip("How quickly the slot lerps between its normal and selected size.")]
    [SerializeField] private float scaleSpeed = 12f;

    [Header("Selection - Arrow")]
    [Tooltip("Arrow prefab spawned below the selected slot.")]
    [SerializeField] private GameObject arrowPrefab;

    [Tooltip("Additional position adjustment for the arrow.")]
    [SerializeField] private Vector2 arrowOffset = new Vector2(0f, -20f);

    [Tooltip("Width assigned to the spawned arrow RectTransform.")]
    [SerializeField] private float arrowWidth = 32f;

    [Tooltip("Height assigned to the spawned arrow RectTransform.")]
    [SerializeField] private float arrowHeight = 32f;

    [Header("Delete")]
    [Tooltip("Hide Delete while this slot is empty.")]
    [SerializeField] private bool hideDeleteButtonWhenEmpty = true;

    [Header("Scene Loading")]
    [SerializeField] private string gameSceneName;
    
    private bool isSelected;

    private Vector3 normalScale;

    private GameObject arrowInstance;

    private static readonly List<SaveMenuButton> buttons = new List<SaveMenuButton>();

    private static int selectedIndex = 0;

    public int SlotIndex
    {
        get
        {
            return (int)buttonType;
        }
    }

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
        /*
        Remember the slot's original scale.
        
        This means the slot can already have a custom scale in
        the Inspector and the selection animation will respect it.
        */
        normalScale = transform.localScale;


        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeleteSave);
    }

    private void OnEnable()
    {
        if (!buttons.Contains(this))
            buttons.Add(this);

        SortButtons();

        RefreshSlotDisplay();

        SelectDefaultButton();
    }

    private void OnDisable()
    {
        buttons.Remove(this);

        RemoveArrow();

        // Reset scale in case the menu gets disabled while this slot is selected.
        transform.localScale = normalScale;

        isSelected = false;
    }

    private void OnDestroy()
    {
        if (deleteButton != null)
            deleteButton.onClick.RemoveListener(DeleteSave);

        RemoveArrow();
    }

    private void Update()
    {
        // Only one SaveMenuButton needs to process the shared keyboard input.
        
        if (buttons.Count == 0 || buttons[0] != this)
            return;

        // Don't move around the save menu while typing a name.
        
        if (SaveNamePopup.IsOpen)
            return;

        HandleKeyboardSelection();
        HandleConfirmInput();
    }

    private void LateUpdate()
    {
        // Selected slot gradually becomes larger.
       
        Vector3 targetScale = isSelected
            ? normalScale * selectedScale
            : normalScale;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }
    
    // Navigation
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
    
    // Slot Activation
    public void ActivateButton()
    {
        if (SaveNamePopup.IsOpen)
            return;

        SelectThisButton();

        /*
        Existing slot:
        load it.
        
        Empty slot:
        ask for a name first.
        */

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
            Debug.LogError("No SaveNamePopup assigned to " + gameObject.name);

            return;
        }

        saveNamePopup.OpenForNewSave(this);
    }
    
    // Rename
    public void BeginRename()
    {
        // "New Game" on an empty slot is only placeholder text.
        // It cannot be renamed because no save exists yet.
        if (!HasSave)
            return;

        if (saveNamePopup == null)
        {
            Debug.LogError( "No SaveNamePopup assigned to " + gameObject.name);

            return;
        }

        SelectThisButton();

        saveNamePopup.OpenForRename(this);
    }
    
    // Create Save
    public void CreateNewSave(string enteredName)
    {
        string finalName =NormalizeSaveName(enteredName);

        PlayerPrefs.SetInt(GetExistsKey(), 1);

        PlayerPrefs.SetString(GetNameKey(), finalName);

        PlayerPrefs.Save();

        RefreshSlotDisplay();

        Debug.Log("Created save slot " +(SlotIndex + 1) + " with the name: " + finalName);

        StartNewGame();
    }
    
    // Rename Save 
    public void RenameSave(string enteredName)
    {
        if (!HasSave)
            return;

        string finalName = NormalizeSaveName(enteredName);

        PlayerPrefs.SetString(GetNameKey(),finalName);

        PlayerPrefs.Save();

        RefreshSlotDisplay();

        Debug.Log("Renamed save slot " + (SlotIndex + 1) + " to: " + finalName);
    }
    
    // Delete Save
    public void DeleteSave()
    {
        if (!HasSave)
        {
            RefreshSlotDisplay();
            return;
        }

        PlayerPrefs.DeleteKey(GetExistsKey());

        PlayerPrefs.DeleteKey(GetNameKey());


        // If just deleted the save that was marked active, remove the active-save information too.
        if (PlayerPrefs.GetInt("ActiveSaveSlot", -1) == SlotIndex)
        {
            PlayerPrefs.DeleteKey("ActiveSaveSlot");

            PlayerPrefs.DeleteKey("ActiveSaveIsNew");
        }

        // Later, when there's actual save data, delete that file here as well.
        DeleteActualSlotData();

        PlayerPrefs.Save();

        RefreshSlotDisplay();

        Debug.Log("Deleted save slot " + (SlotIndex + 1) + ".");
    }

    private void DeleteActualSlotData()
    {
        ChestStateSave.ClearSlot(SlotIndex);
        // Placeholder for future actual save-file deletion.
    }
    
    // Scene Loading
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
            Debug.LogWarning("No game scene assigned to " + gameObject.name);

            return;
        }

        // Keep using your existing transition system.
        if (RoomTransitionManager.Instance == null)
        {
            Debug.LogError("No RoomTransitionManager instance exists.");

            return;
        }

        RoomTransitionManager.Instance.BeginTransition(gameSceneName);
    }
    
    // Display
    public void RefreshSlotDisplay()
    {
        if (saveNameText != null)
        {
            saveNameText.gameObject.SetActive(true);

            saveNameText.text = HasSave
                ? CurrentSaveName
                : "New Game";
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
            deleteButton.gameObject.SetActive(true);

            deleteButton.interactable = HasSave;
        }
    }
    
    // Pointer Input
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SaveNamePopup.IsOpen)
            return;

        SelectThisButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (SaveNamePopup.IsOpen)
            return;

        // Name and Delete have their own click behavior.
        // If Unity passes their click to this parent too, don't activate the slot.
        if (WasNameClicked(eventData))
            return;

        if (WasDeleteClicked(eventData))
            return;

        // Clicking Background or Frame reaches SaveMenuButton and activates it
        ActivateButton();
    }

    private bool WasNameClicked(PointerEventData eventData)
    {
        if (saveNameText == null)
            return false;

        GameObject clicked = eventData.pointerPressRaycast.gameObject;

        if (clicked == null)
            return false;

        Transform clickedTransform = clicked.transform;

        return
            clickedTransform == saveNameText.transform || clickedTransform.IsChildOf(saveNameText.transform);
    }

    private bool WasDeleteClicked(PointerEventData eventData)
    {
        if (deleteButton == null)
            return false;

        GameObject clicked = eventData.pointerPressRaycast.gameObject;

        if (clicked == null)
            return false;

        Transform clickedTransform = clicked.transform;

        return
            clickedTransform == deleteButton.transform || clickedTransform.IsChildOf(deleteButton.transform);
    }
    
    // Selection
    public void SelectThisButton()
    {
        int index = buttons.IndexOf(this);

        if (index >= 0)
            SelectButton(index);
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

        if (isSelected)
        {
            SpawnArrow();
        }
        else
        {
            RemoveArrow();
        }
    }

    private static void SelectDefaultButton()
    {
        if (buttons.Count == 0)
            return;

        SaveMenuButton firstSlot = buttons.Find(button => button.buttonType ==ButtonType.SlotOne);

        if (firstSlot != null)
        {
            SelectButton(buttons.IndexOf(firstSlot));
        }
        else
        {
            SelectButton(0);
        }
    }
    
    // Arrow
    private void SpawnArrow()
    {
        RemoveArrow();

        if (arrowPrefab == null)
            return;

        RectTransform slotRect = transform as RectTransform;

        if (slotRect == null)
        {
            Debug.LogError(gameObject.name + " needs a RectTransform.");

            return;
        }


        // Same spawn method as the other menu selection arrows: instantiate directly underneath the selected UI object.
        arrowInstance = Instantiate(arrowPrefab, transform);

        RectTransform arrowRect = arrowInstance.GetComponent<RectTransform>();

        if (arrowRect == null)
        {
            Debug.LogError("Arrow prefab needs a RectTransform.");

            Destroy(arrowInstance);
            arrowInstance = null;

            return;
        }


        arrowInstance.SetActive(true);


        // Center anchor lets us calculate its position relative to the center of the slot.
        arrowRect.anchorMin = new Vector2(0.5f, 0.5f);

        arrowRect.anchorMax = new Vector2(0.5f, 0.5f);

        arrowRect.pivot = new Vector2(0.5f, 0.5f);


        arrowRect.sizeDelta = new Vector2(arrowWidth, arrowHeight);


        /*
        Position at the bottom edge of the slot.
        
        arrowOffset.x moves it left/right.
        arrowOffset.y moves it farther above/below the edge.
        */
        float slotHalfHeight = slotRect.rect.height / 2f;

        arrowRect.anchoredPosition = new Vector2(arrowOffset.x, -slotHalfHeight + arrowOffset.y);


        arrowRect.localScale = Vector3.one;

        arrowRect.localRotation = Quaternion.identity;


        // Keep the arrow visually above Background and Frame.
        arrowRect.SetAsLastSibling();
    }

    private void RemoveArrow()
    {
        if (arrowInstance == null)
            return;

        Destroy(arrowInstance);

        arrowInstance = null;
    }

    // HELPERS

    private string NormalizeSaveName(
        string enteredName
    )
    {
        if (string.IsNullOrWhiteSpace(enteredName))
            return "New Game";

        return enteredName.Trim();
    }

    private string GetExistsKey()
    {
        return
            "SaveSlot_" + SlotIndex + "_Exists";
    }

    private string GetNameKey()
    {
        return
            "SaveSlot_" + SlotIndex + "_Name";
    }

    private static void SortButtons()
    {
        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(
            b.transform.GetSiblingIndex())
        );
    }
}