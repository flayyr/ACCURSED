using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    public static ItemInfoPanel Instance { get; private set; }

    [SerializeField] public GameObject itemName;
    [SerializeField] public GameObject itemType;
    [SerializeField] public GameObject labelingText;
    [SerializeField] public GameObject numberHeld;
    [SerializeField] public GameObject itemDesc;

    public Inventory_ItemSO currentItemDisplay;
    [SerializeField] private Image itemDisplay;

    private void Awake()
    {

        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //Debug.Log("MY NAME IS INFO PANEL AND I EXIST");

        UpdateDisplay(null, 0);

    }
    public void UpdateDisplay(Inventory_ItemSO item, int quantity)
    {
        currentItemDisplay = item;

        if (item != null) {
            Debug.Log(item.name);
            labelingText.SetActive(true);

            itemName.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemName;
            itemType.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemType;
            //itemQuantity.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.currentItemDisplay..ToString() + " / " + currentItemDisplay.itemQuantityMax.ToString();
            numberHeld.GetComponent<TextMeshProUGUI>().text = quantity.ToString() + " / " + currentItemDisplay.itemQuantityMax.ToString();
            itemDesc.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemDesc;
        }
        else
        {
            //Debug.Log("null");
            labelingText.SetActive(false);

            itemName.GetComponent<TextMeshProUGUI>().text = "";
            itemType.GetComponent<TextMeshProUGUI>().text = "";
            //itemQuantity.GetComponent<TextMeshProUGUI>().text = "";
            numberHeld.GetComponent<TextMeshProUGUI>().text = "";
            itemDesc.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void Display(ItemPickupSO item, int totalHeld)
    {
        if (item == null)
            return;

        currentItemDisplay = null;

        if (labelingText != null)
            labelingText.SetActive(true);

        SetText(itemName, item.itemName);

        SetText(itemType, item.itemCategory.ToString());

        SetText(labelingText, "No. Held: " + totalHeld);

        SetText(itemDesc, item.itemDesc);

        if (itemDisplay != null)
        {
            itemDisplay.sprite = item.itemSpr;
            itemDisplay.enabled = item.itemSpr != null;
        }
    }

    private void SetText(GameObject target, string value)
    {
        if (target == null)
            return;

        TMP_Text text = target.GetComponent<TMP_Text>();

        if (text != null)
            text.text = value;
    }
}
