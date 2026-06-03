using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

// This class manages the specific Prefab for normal item pickups
public class NormalItemPickupUI : MonoBehaviour
{
    [SerializeField] ItemPickupSO item;

    [SerializeField] GameObject imgUI;
    [SerializeField] GameObject nameUI;
    [SerializeField] GameObject quantityUI;

    /*public NormalItemPickupUI(ItemPickupSO item)
    {
        this.item = item;
        this.imgUI.GetComponent<Image>().sprite = item.itemSpr;
        this.nameUI.GetComponent <TextMeshProUGUI>().text = item.itemName;
        quantityUI.GetComponent<TextMeshProUGUI>().text = (item.itemQuantity != 0) ? "x" + item.itemQuantity : "x1";
    }*/

    // Monobehaviors can't use constructors apparently so here's an initialize method instead
    public void Initialize(ItemPickupSO item)
    {
        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;
        quantityUI.GetComponent<TextMeshProUGUI>().text = item.itemQuantity > 0 ? $"x{item.itemQuantity}" : "x1";
    }

    private void Awake()
    {
        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;

        quantityUI.GetComponent<TextMeshProUGUI>().text = (item.itemQuantity != 0) ? $"x{item.itemQuantity}" : "x1";

    }

}
