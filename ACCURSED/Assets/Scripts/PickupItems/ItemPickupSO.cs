using UnityEngine;

[CreateAssetMenu(fileName = "ItemPickupSO", menuName = "Scriptable Objects/ItemPickupSO")]
public class ItemPickupSO : ScriptableObject
{
    public Sprite itemSpr;
    public string itemName;
    public string itemDesc;
    public bool isSpecialItem;

    public GameObject promptUIObj;
    public GameObject itemShowcaseUIObj;
}
