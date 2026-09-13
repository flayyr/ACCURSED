using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory_ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    [SerializeField] public GameObject itemSlot;
    [SerializeField] public GameObject itemSprDisplay;
    [SerializeField] public GameObject itemBox;
    //[SerializeField] public GameObject infoPanel;
    [SerializeField] public GameObject quantityText;
    [SerializeField] public GameObject selectedHue;

    private Inventory_ItemSO item;
    private bool isEmpty;
    ButtonHighlight highlight;
    private int quantity;
    bool isMouseHovering;

    public static GameObject selectedSlot;
    public static GameObject rightClickOptions;

    public Button b;
    void Awake()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(ExecuteTask);

        highlight = itemSlot.GetComponent<ButtonHighlight>();

        selectedHue.SetActive(false);
        selectedSlot = null;
        isMouseHovering = false;
    }

    // Registers there is an item in this item slot
    public void SetItem(Inventory_ItemSO newItem)
    {
        item = newItem;

        if (item != null)
        {
            isEmpty = false;

            Image itemImage = itemSprDisplay.GetComponent<Image>();

            itemImage.sprite = item.itemSpr;
            itemImage.color = Color.white;
            itemBox.GetComponent<Image>().color = new Color32(30, 30, 30, 255);
        }
        else
        {
            isEmpty = true;

            Image itemImage = itemSprDisplay.GetComponent<Image>();

            itemImage.sprite = null;
            itemImage.color = new Color(0f, 0f, 0f, 0f);
            itemBox.GetComponent<Image>().color = Color.black;
        }
    }

    // Executes when the ItemSlot is left-clicked, updates the Info Panel
    void ExecuteTask()
    {
        if (!isEmpty && !CheckIfSelected()) {
            //Debug.Log("Update Item Before");
            ItemInfoPanel.Instance.UpdateDisplay(item, quantity);
            SetSelected();
            //Debug.Log("Update Item");
        }
    }

    // Makes sure the selected item 
    void UpdateActivity()
    {
        if (isEmpty)
        {
            quantityText.SetActive(false);
            highlight.isEnabled = false;
        }
        else
        {
            quantityText.SetActive(true);
            highlight.isEnabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActivity();
        ManageSelectionProperties();
    }

    public Inventory_ItemSO GetItem()
    {
        return item;
    }

    public bool CheckIfEmpty()
    {
        return isEmpty;
    }

    public void SetIfEmpty(bool b)
    {
        this.isEmpty = b;
    }

    public void SetQuantity(int quantity)
    {
        quantityText.GetComponent<TextMeshProUGUI>().text = quantity.ToString();
        this.quantity = quantity;
    }

    public bool CheckIfSelected()
    {
        return itemSlot.Equals(selectedSlot);
    }

    public void SetSelected()
    {
        selectedSlot = itemSlot;
    }

    public void ManageSelectionProperties()
    {
        if (CheckIfSelected())
        {
            selectedHue.SetActive(true);
        }
        else
        {
            selectedHue.SetActive(false);
        }
    }

    //Right Click

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseHovering = false;
    }

    public void CheckRightClick()
    {
        if (isMouseHovering && Input.GetMouseButtonDown(1)) {
            RightClickOptions.Instance.GetIsOpen();
        }
    }
}
