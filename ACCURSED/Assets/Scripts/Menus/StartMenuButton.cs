using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonType
    {
        Play,
        Saves,
        Settings,
        Credits,
        Quit
    }

    [Header("Button Type")]
    public ButtonType buttonType;

    [Header("Scene Names")]
    public string startScreenName = "StartMenu";
    public string playName = "PlayScene";
    public string saveName = "SavesScene";
    public string settingName = "SettingScene";
    public string creditName = "CreditScene";

    [Header("Selection Visualization")]
    public float selectedScale = 1.2f;
    public float scaleSpeed = 12f;

    [Header("Arrows")]
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;

    public Vector3 leftArrowOffset = new Vector3(-120f, 0, 0);
    public Vector3 rightArrowOffset = new Vector3(120f, 0, 0);

    [Header("Inputs")]
    public bool allowEscapeToQuit = true;

    private Vector3 normalScale;
    private GameObject leftArrowInstance;
    private GameObject rightArrowInstance;
    private bool isSelected;

    private static readonly List<StartMenuButton> buttons = new List<StartMenuButton>();
    private static int selectedIndex = 0;

    private void Awake()
    {
        normalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (!buttons.Contains(this))
            buttons.Add(this);

        SortButtons();

        SelectDefaultPlayButton();
    }

    private void OnDisable()
    {
        buttons.Remove(this);

        if (leftArrowInstance  != null)
            Destroy(leftArrowInstance);

        if (rightArrowInstance != null)
            Destroy(rightArrowInstance);
    }

    public void Update()
    {
        if (buttons.Count == 0 || buttons[0] != this)
            return;

        HandleKeyboardSelection();
        HandleConfirmInput();
        HandleEscapeInput();
    }

    private void LateUpdate()
    {
        Vector3 targetScale = isSelected ? normalScale * selectedScale : normalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    private void HandleKeyboardSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SelectButton(selectedIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            SelectButton(selectedIndex - 1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            SelectButton(selectedIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            SelectButton(selectedIndex - 1);
        }
    }

    private void HandleConfirmInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedIndex >= 0 && selectedIndex < buttons.Count)
            {
                buttons[selectedIndex].ActivateButton();
            }
        }
    }

    private void HandleEscapeInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != startScreenName)
        {
            SceneManager.LoadScene(startScreenName);
            return;
        }

        if (allowEscapeToQuit)
        {
            StartMenuButton quitButton = buttons.Find(b => b.buttonType == ButtonType.Quit);

            if (quitButton != null)
            {
                quitButton.ActivateButton();
            }
        }
    }

    public void ActivateButton()
    {
        switch (buttonType)
        {
            case ButtonType.Play:
                SceneManager.LoadScene(playName);
                break;

            case ButtonType.Saves:
                SceneManager.LoadScene(saveName);
                break;

            case ButtonType.Settings:
                SceneManager.LoadScene(settingName);
                break;

            case ButtonType.Credits:
                SceneManager.LoadScene(creditName);
                break;

            case ButtonType.Quit:
                QuitGame();
                break;
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

    private static void SelectDefaultPlayButton()
    {
        if (buttons.Count == 0)
            return;

        StartMenuButton playButton = buttons.Find(b => b.buttonType == ButtonType.Play);

        if (playButton != null)
        {
            int playIndex = buttons.IndexOf(playButton);
            SelectButton(playIndex);
        }
        else
        {
            SelectButton(0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered: " + gameObject.name);

        int index = buttons.IndexOf(this);

        if (index != -1)
        {
            SelectButton(index);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

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
            SpawnArrows();
        }
        else
        {
            RemoveArrows();
        }
    }

    private void SpawnArrows()
    {
        RemoveArrows();

        RectTransform buttonRect = GetComponent<RectTransform>();

        if (buttonRect == null)
        {
            Debug.LogWarning("Button has no RectTransform: " + gameObject.name);
            return;
        }

        if (leftArrowPrefab != null)
        {
            leftArrowInstance = Instantiate(leftArrowPrefab, transform);
            SetupArrowAsButtonChild(leftArrowInstance, buttonRect, true);
        }

        if (rightArrowPrefab != null)
        {
            rightArrowInstance = Instantiate(rightArrowPrefab, transform);
            SetupArrowAsButtonChild(rightArrowInstance, buttonRect, false);
        }
    }

    private void SetupArrowAsButtonChild(GameObject arrowObject, RectTransform buttonRect, bool isLeftArrow)
    {
        RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();

        if (arrowRect == null)
        {
            Debug.LogWarning("Arrow prefab needs a RectTransform.");
            return;
        }

        arrowObject.SetActive(true);

        arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);

        float buttonHalfWidth = buttonRect.rect.width / 2f;

        Vector3 offset = isLeftArrow ? leftArrowOffset : rightArrowOffset;

        float baseX = isLeftArrow ? -buttonHalfWidth : buttonHalfWidth;

        arrowRect.anchoredPosition = new Vector2(
            baseX + offset.x,
            offset.y
        );

        arrowRect.localScale = Vector3.one;
        arrowRect.localRotation = Quaternion.identity;

        arrowRect.SetAsLastSibling();
    }

    private void RemoveArrows()
    {
        if (leftArrowInstance != null)
        {
            Destroy(leftArrowInstance);
            leftArrowInstance = null;
        }

        if (rightArrowInstance != null)
        {
            Destroy(rightArrowInstance);
            rightArrowInstance = null;
        }
    }

    private static void SortButtons()
    {
        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }
}
