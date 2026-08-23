using UnityEngine;
using UnityEngine.UI;

public class Inventory_ItemSlot : MonoBehaviour
{
    
    [SerializeField] public GameObject itemSlot;
    [SerializeField] public GameObject itemSprDisplay;
    [SerializeField] public GameObject infoPanel;

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
            highlight.isEnabled = false;
        }
        else
        {
            highlight.isEnabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActivity();
    }

}
