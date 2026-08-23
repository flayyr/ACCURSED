using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    [SerializeField] public GameObject itemName;
    [SerializeField] public GameObject itemType;
    [SerializeField] public GameObject labelingText;
    [SerializeField] public GameObject itemQuantity;
    [SerializeField] public GameObject itemDesc;

    public Inventory_ItemSO currentItemDisplay;

    private void Awake()
    {
        UpdateDisplay(null);
    }
    public void UpdateDisplay(Inventory_ItemSO item)
    {
        currentItemDisplay = item;

        if (item != null) {
            labelingText.SetActive(true);

            itemName.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemName;
            itemType.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemType;
            itemQuantity.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemQuantity.ToString() + " / " + currentItemDisplay.itemQuantityMax.ToString();
            itemDesc.GetComponent<TextMeshProUGUI>().text = currentItemDisplay.itemDesc;
        }
        else
        {
            labelingText.SetActive(false);

            itemName.GetComponent<TextMeshProUGUI>().text = "";
            itemType.GetComponent<TextMeshProUGUI>().text = "";
            itemQuantity.GetComponent<TextMeshProUGUI>().text = "";
            itemDesc.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

  


}
