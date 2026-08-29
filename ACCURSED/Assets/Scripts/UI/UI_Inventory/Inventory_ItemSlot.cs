using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory_ItemSlot : MonoBehaviour
{
    
    [SerializeField] public GameObject itemSlot;
    [SerializeField] public GameObject itemSprDisplay;
    [SerializeField] public GameObject infoPanel;
    [SerializeField] public GameObject quantityText;

    private Inventory_ItemSO item;
    private bool isEmpty;
    ButtonHighlight highlight;

    public Button b;
    void Start()
    {
        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(ExecuteTask);
        highlight = itemSlot.gameObject.GetComponent<ButtonHighlight>();

        if (item != null)
        {
            isEmpty = false;
            itemSprDisplay.GetComponent<Image>().sprite = item.itemSpr;
            
        }
        else
        {
            isEmpty = true;
            itemSprDisplay.GetComponent<Image>().sprite = null;
            itemSprDisplay.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        }
    }

    void ExecuteTask()
    {
        infoPanel.GetComponent<ItemInfoPanel>().UpdateDisplay(item);
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
    }

    public Inventory_ItemSO GetItem()
    {
        return item;
    }

    public void SetItem(Inventory_ItemSO item)
    {
        this.item = item;
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

}
