using UnityEngine;

[CreateAssetMenu(fileName = "ItemPickupSO", menuName = "Scriptable Objects/ItemPickupSO")]
public class ItemPickupSO : ScriptableObject
{
    public Sprite itemSpr;
    public string itemName;
    public string itemDesc;
    public bool isSpecialItem;
    public int itemQuantity; // if isSpecialItem == true, this has cannot be greater than 1

    public GameObject promptUIObj;
    public GameObject itemShowcaseUIObj;
}
