using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public static StartMenuManager Instance { get; private set; }

    [Header("Menu Buttons")]
    public List<StartMenuButton> buttons = new List<StartMenuButton>();

    [Header("Scene Names")]
    public string startScreenName = "StartMenu";
    public string playName = "PlayScene";
    public string saveName = "SavesScene";
    public string creditName = "CreditScene";

    [Header("Start Button")]
    public TextMeshProUGUI buttonText;
    public bool gameExist = false;
    [SerializeField] private string Continue = "Continue";
    [SerializeField] private string NewGame = "New Game";

    [Header("Settings Panel")]
    public GameObject settingsPanel;

    [SerializeField] private SettingsPrefabSpawner settingsSpawner;
    [SerializeField] private SettingsMenuNavigator settingsNavigator;

    [Tooltip("Optional. Assign the first settings button/slider you want selected when the panel opens.")]
    public GameObject firstSettingsSelectedObject;

    [Tooltip("Optional. Assign the first main menu button to reselect after settings closes.")]
    public GameObject firstMainMenuSelectedObject;

    [Header("Input")]
    public bool allowHorizontalNavigation = true;

    private int selectedIndex = 0;
    private bool settingsOpen = false;
    private bool inputBlockedBySettings;

    private void Awake()
    {
        buttons.RemoveAll(button => button == null);

        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].Initialize(this);
        }

        if (settingsSpawner == null)
            settingsSpawner = FindFirstObjectByType<SettingsPrefabSpawner>();

        if (settingsNavigator == null && settingsPanel != null)
            settingsNavigator = settingsPanel.GetComponentInChildren<SettingsMenuNavigator>(true);
    }

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        SelectDefaultPlayButton();
    }

    private void Update()
    {
        

        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "StartMenu")
            Destroy(gameObject);

        if (inputBlockedBySettings)
            return;

        if (settingsOpen)
        {
            HandleSettingsInput();
            return;
        }
        
        HandleMainMenuInput();
    }

    private void HandleMainMenuInput()
    {
        if (buttons.Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectByIndex(selectedIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectByIndex(selectedIndex - 1);
        }

        if (allowHorizontalNavigation)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectByIndex(selectedIndex + 1);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectByIndex(selectedIndex - 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ActivateSelectedButton();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeFromMainMenu();
        }
    }

    private void HandleSettingsInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // Escape is currently being captured as a keybind,
        // or it was captured earlier during this same frame.
        if (settingsNavigator != null)
            return;

        CloseSettingsPanel();
    }

    private void HandleEscapeFromMainMenu()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != startScreenName)
        {
            SceneManager.LoadScene(startScreenName);
        }
    }

    private void ActivateSelectedButton()
    {
        if (selectedIndex < 0 || selectedIndex >= buttons.Count)
            return;

        ActivateButton(buttons[selectedIndex]);
    }

    public void ActivateButton(StartMenuButton button)
    {
        if (button == null)
            return;

        if (settingsOpen)
            return;

        switch (button.buttonType)
        {
            case StartMenuButton.ButtonType.Play:
                RoomTransitionManager.Instance.BeginTransition(playName);
                break;

            case StartMenuButton.ButtonType.Saves:
                RoomTransitionManager.Instance.BeginTransition(saveName);
                break;

            case StartMenuButton.ButtonType.Settings:
                settingsSpawner.OpenSettings();
                break;

            case StartMenuButton.ButtonType.Credits:
                RoomTransitionManager.Instance.BeginTransition(creditName);
                break;

            case StartMenuButton.ButtonType.Quit:
                QuitGame();
                break;
        }
    }

    public void SelectButton(StartMenuButton button)
    {
        if (settingsOpen)
            return;

        int index = buttons.IndexOf(button);

        if (index == -1)
            return;

        SelectByIndex(index);
    }

    private void SelectByIndex(int index)
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

        if (buttons[selectedIndex] != null)
        {
            EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);
        }
    }

    private void SelectDefaultPlayButton()
    {
        if (buttons.Count == 0)
            return;

        StartMenuButton playButton = buttons.Find(
            button => button.buttonType == StartMenuButton.ButtonType.Play
        );

        if (playButton != null)
        {
            SelectByIndex(buttons.IndexOf(playButton));
        }
        else
        {
            SelectByIndex(0);
        }
    }

    public void ChangeContinueOrNewGame()
    {
        if (gameExist)
        {
            buttonText.text = Continue;
        }
        else
        {
            buttonText.text = NewGame;
        }
    }

    private void OpenSettingsPanel()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("Settings Panel is not assigned.");
            return;
        }

        if (settingsNavigator == null)
            settingsNavigator = settingsPanel.GetComponentInChildren<SettingsMenuNavigator>(true);

        settingsOpen = true;

        ClearMainMenuSelectionVisuals();

        settingsPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        GameObject objectToSelect = firstSettingsSelectedObject;

        if (objectToSelect == null)
        {
            Selectable firstSelectable = settingsPanel.GetComponentInChildren<Selectable>(true);

            

            if (firstSelectable != null)
                objectToSelect = firstSelectable.gameObject;
        }

        if (objectToSelect != null)
        {
            EventSystem.current.SetSelectedGameObject(objectToSelect);
            Debug.Log("Selected settings object: " + objectToSelect.name);
        }
        else
        {
            Debug.LogWarning("No selectable object found inside Settings Panel.");
        }
    }

    private void CloseSettingsPanel()
    {
        settingsOpen = false;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        EventSystem.current.SetSelectedGameObject(null);

        SelectDefaultPlayButton();
    }

    private void ClearMainMenuSelectionVisuals()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].SetSelected(false);
            }
        }
    }

    public void SetSettingsInputBlocked(bool blocked)
    {
        if (inputBlockedBySettings == blocked)
            return;

        inputBlockedBySettings = blocked;

        if (inputBlockedBySettings)
        {
            ClearMainMenuSelectionVisuals();

            if (EventSystem.current != null)
            {
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

                if (selectedObject != null && selectedObject.transform.IsChildOf(transform))
                    EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else
        {
            SelectDefaultPlayButton();
        }
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}