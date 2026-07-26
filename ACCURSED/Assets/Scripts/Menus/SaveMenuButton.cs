using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    private static int selectedIndex = 0;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
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
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectButton(selectedIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectButton(selectedIndex - 1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectButton(selectedIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) )
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
            case ButtonType.SlotOne:
                //for save function
                Debug.Log(gameObject.name + " pressed");
                break;

            case ButtonType.SlotTwo:
                //for save function
                Debug.Log(gameObject.name + " pressed");
                break;

            case ButtonType.SlotThree:
                //for save function
                Debug.Log(gameObject.name + " pressed");
                break;

            case ButtonType.SlotFour:
                //for save function
                Debug.Log(gameObject.name + " pressed");
                break;
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

        ChangeSprite();
    }

    private static void SelectDefaultPlayButton()
    {
        if (buttons.Count == 0)
            return;

        SaveMenuButton playButton = buttons.Find(b => b.buttonType == ButtonType.SlotOne);

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

    void ChangeSprite()
{
    if (buttonImage == null)
    {
        Debug.LogError("No Image component found on " + gameObject.name);
        return;
    }

    switch (buttonType)
    {
        case ButtonType.SlotOne:
            buttonImage.sprite = isSelected ? slotOneSelectedSprite : slotOneNormalSprite;
            break;

        case ButtonType.SlotTwo:
            buttonImage.sprite = isSelected ? slotTwoSelectedSprite : slotTwoNormalSprite;
            break;

        case ButtonType.SlotThree:
            buttonImage.sprite = isSelected ? slotThreeSelectedSprite : slotThreeNormalSprite;
            break;

        case ButtonType.SlotFour:
            buttonImage.sprite = isSelected ? slotFourSelectedSprite : slotFourNormalSprite;
            break;
    }
}

    private static void SortButtons()
    {
        buttons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }
}
