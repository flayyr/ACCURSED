using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

// This class manages the specific Prefab for normal item pickups
public class SpecialItemPickupUI : MonoBehaviour
{
    [SerializeField] ItemPickupSO item;

    [SerializeField] GameObject imgUI;
    [SerializeField] GameObject nameUI;
    [SerializeField] GameObject descUI;

    private void Awake()
    {
        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;
        descUI.GetComponent<TextMeshProUGUI>().text = item.itemDesc;
    }

}
