using UnityEngine;

[CreateAssetMenu(fileName = "ItemPickupSO", menuName = "Scriptable Objects/ItemPickupSO")]
public class ItemPickupSO : ScriptableObject
{
    public Sprite itemSpr;
    public string itemName;
    public string itemDesc;
    public bool isSpecialItem;
    public int itemQuantity = 1; // WIP: to be implemented to be assigned from TooltipManager, not manually

    //public GameObject promptUIObj;
    public GameObject itemShowcaseUIObj;
}
