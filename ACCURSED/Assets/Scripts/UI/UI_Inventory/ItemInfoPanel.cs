using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    public static ItemInfoPanel Instance { get; private set; }

    [SerializeField] public GameObject itemName;
    [SerializeField] public GameObject itemType;
    [SerializeField] public GameObject labelingText;
    [SerializeField] public GameObject itemDesc;

    public Inventory_ItemSO currentItemDisplay;

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

        UpdateDisplay(null);

    }
    public void UpdateDisplay(Inventory_ItemSO item)
    {
        currentItemDisplay = item;

        if (item != null) {
            Debug.Log(item.name);
            labelingText.SetActive(true);

            itemName.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemName;
            itemType.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemType;
            //itemQuantity.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.currentItemDisplay..ToString() + " / " + currentItemDisplay.itemQuantityMax.ToString();
            itemDesc.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemDesc;
        }
        else
        {
            Debug.Log("null");
            labelingText.SetActive(false);

            itemName.GetComponent<TextMeshProUGUI>().text = "";
            itemType.GetComponent<TextMeshProUGUI>().text = "";
            //itemQuantity.GetComponent<TextMeshProUGUI>().text = "";
            itemDesc.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

  


}
