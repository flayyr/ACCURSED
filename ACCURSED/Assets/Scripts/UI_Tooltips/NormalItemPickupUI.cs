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

    private void Awake()
    {
        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;
    }

}
