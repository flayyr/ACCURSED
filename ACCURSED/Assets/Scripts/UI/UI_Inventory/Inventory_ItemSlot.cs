using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory_ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
    
    [Header("UI")]
    [SerializeField] private Image itemIcon;

    [SerializeField] private ItemInfoPanel infoPanel;
    private InventoryStack stack;
    private PlayerInventory inventory;

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
        /*
        if (!isEmpty && !CheckIfSelected()) {
            //Debug.Log("Update Item Before");
            ItemInfoPanel.Instance.UpdateDisplay(item, quantity);
            SetSelected();
            //Debug.Log("Update Item");
        }
        */
        
        if (isEmpty)
            return;

        if (stack == null || stack.item == null)
            return;

        SetSelected();

        ItemInfoPanel targetInfoPanel = infoPanel != null
            ? infoPanel
            : ItemInfoPanel.Instance;

        if (targetInfoPanel != null)
        {
            int totalHeld = inventory != null
                ? inventory.GetQuantity(stack.item.itemID)
                : stack.quantity;

            targetInfoPanel.Display(stack.item, totalHeld);
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

        if (quantityText != null)
        {
            quantityText.SetActive(!isEmpty);
        }

        if (highlight != null)
        {
            highlight.isEnabled = !isEmpty;
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

    public InventoryStack GetStack()
    {
        return stack;
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
        if (itemSlot == null)
            return false;

        return itemSlot.Equals(selectedSlot);
    }

    public void SetSelected()
    {
        //selectedSlot = itemSlot;

        if (itemSlot != null)
        {
            selectedSlot = itemSlot;
        }
        else
        {
            selectedSlot = gameObject;
        }
    }

    public void ManageSelectionProperties()
    {
        if (selectedHue == null)
            return;

        if (CheckIfSelected())
        {
            selectedHue.SetActive(true);
        }
        else
        {
            selectedHue.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseHovering = false;
    }

    public void Bind(InventoryStack newStack, PlayerInventory newInventory, ItemInfoPanel infoPanelOverride = null)
    {
        stack = newStack;
        inventory = newInventory;

        if (infoPanelOverride != null)
            infoPanel = infoPanelOverride;

        isEmpty = stack == null || stack.item == null;

        RefreshVisual();
        UpdateActivity();
    }

    private void RefreshVisual()
    {
        if (isEmpty)
        {
            ClearVisual();
            return;
        }

        ItemPickupSO item = stack.item;

        if (itemSprDisplay != null)
        {
            Image itemImage = itemSprDisplay.GetComponent<Image>();

            if (itemImage != null)
            {
                itemImage.sprite = item.itemSpr;
                itemImage.color = item.itemSpr != null
                    ? Color.white
                    : new Color(0f, 0f, 0f, 0f);
            }
        }

        if (itemBox != null)
        {
            Image boxImage = itemBox.GetComponent<Image>();

            if (boxImage != null)
                boxImage.color = new Color32(30, 30, 30, 255);
        }

        if (quantityText != null)
        {
            TMP_Text text = quantityText.GetComponent<TMP_Text>();

            if (text != null)
            {
                text.text = stack.quantity.ToString();
            }
        }
    }

    private void ClearVisual()
    {
        if (itemSprDisplay != null)
        {
            Image itemImage = itemSprDisplay.GetComponent<Image>();

            if (itemImage != null)
            {
                itemImage.sprite = null;
                itemImage.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        if (itemBox != null)
        {
            Image boxImage = itemBox.GetComponent<Image>();

            if (boxImage != null)
                boxImage.color = Color.black;
        }

        if (quantityText != null)
        {
            TMP_Text text = quantityText.GetComponent<TMP_Text>();

            if (text != null)
                text.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEmpty)
            return;

        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        SetSelected();

        if (RightClickOptions.Instance != null)
            RightClickOptions.Instance.GetIsOpen();
    }

    private void SelectItem()
    {
        if (highlight != null)
            highlight.gameObject.SetActive(true);

        if (infoPanel != null)
        {
            int totalHeld = inventory != null
                ? inventory.GetQuantity(stack.item.itemID)
                : stack.quantity;

            infoPanel.Display(stack.item, totalHeld);
        }
    }
}
