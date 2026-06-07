using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingsMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonType
    {
        Keyboard,
        Controller,
        Display,
        Sound,
        Settings,
        Quit
    }

    [Header("Button Type")]
    public ButtonType buttonType;

    [Header("Scene Name")]
    public string startScreenName = "StartMenu";

    [Header("Settings Tabs")]
    public GameObject keyboardTab;
    public GameObject controllerTab;
    public GameObject displayTab;
    public GameObject soundTab;
    public GameObject settingsTab;

    [Header("Selected")]
    public GameObject ArrowPrefab;
    private GameObject ArrowInstance;
    public float ArrowWidth;
    public float ArrowHeight;

    private Vector3 normalScale;
    private bool isSelected;

    private static readonly List<SettingsMenuButton> buttons = new List<SettingsMenuButton>();
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

        if (ArrowInstance != null)
            Destroy(ArrowInstance);
    }

    public void Update()
    {
        if (buttons.Count == 0 || buttons[0] != this)
            return;

        HandleKeyboardSelection();
        HandleConfirmInput();
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

    public void ActivateButton()
    {
        switch (buttonType)
        {
            case ButtonType.Keyboard:
                ShowTab(keyboardTab);
                break;

            case ButtonType.Controller:
                ShowTab(controllerTab);
                break;

            case ButtonType.Display:
                ShowTab(displayTab);
                break;

            case ButtonType.Sound:
                ShowTab(soundTab);
                break;

            case ButtonType.Settings:
                ShowTab(settingsTab);
                break;

            case ButtonType.Quit:
                SceneManager.LoadScene(startScreenName);
                break;
        }
    }

    private void ShowTab(GameObject TabToShow)
    {
        keyboardTab.SetActive(false);
        controllerTab.SetActive(false);
        displayTab.SetActive(false);
        soundTab.SetActive(false);
        settingsTab.SetActive(false);

        TabToShow.SetActive(true);
    }

    private static void SelectDefaultPlayButton()
    {
        if (buttons.Count == 0)
            return;

        SettingsMenuButton playButton = buttons.Find(b => b.buttonType == ButtonType.Keyboard);

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
            SpawnArrow();
        }
        else
        {
            RemoveArrow();
        }
    }

    private void SpawnArrow()
    {
        RemoveArrow();

        RectTransform buttonRect = GetComponent<RectTransform>();

        if (buttonRect == null)
        {
            Debug.LogWarning("Button has no RectTransform: " + gameObject.name);
            return;
        }

        if (ArrowPrefab != null)
        {
            ArrowInstance = Instantiate(ArrowPrefab, transform);
            SetupArrowAsButtonChild(ArrowInstance, buttonRect);
        }
    }

    private void SetupArrowAsButtonChild(GameObject arrowObject, RectTransform buttonRect)
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

        arrowRect.sizeDelta = new Vector2(ArrowWidth, ArrowHeight);

        float buttonHalfWidth = buttonRect.rect.width / 2f;
        float arrowOffset = 60f;

        // This places one arrow to the left of the button.
        arrowRect.anchoredPosition = new Vector2(-buttonHalfWidth + arrowOffset, 0f);

        arrowRect.localScale = Vector3.one;
        arrowRect.localRotation = Quaternion.identity;


        arrowRect.SetAsLastSibling();
    }

    private void RemoveArrow()
    {
        if (ArrowInstance != null)
        {
            Destroy(ArrowInstance);
            ArrowInstance = null;
        }
    }

    private static void SortButtons()
    {
        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }
}
