using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory_ItemSlot : MonoBehaviour
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

    public static GameObject selectedSlot;

    public Button b;
    void Awake()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(ExecuteTask);

        highlight = itemSlot.GetComponent<ButtonHighlight>();

        selectedHue.SetActive(false);
        selectedSlot = null;
    }

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

    void ExecuteTask()
    {
        if (!isEmpty && !CheckIfSelected()) {
            Debug.Log("Update Item Before");
            ItemInfoPanel.Instance.UpdateDisplay(item);
            SetSelected();
            Debug.Log("Update Item");
        }
    }

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


}
