using UnityEngine;

[CreateAssetMenu(fileName = "Inventory_ItemSO",menuName = "Scriptable Objects/Inventory_ItemSO")]
public class Inventory_ItemSO : ScriptableObject
{
    public Sprite itemSpr;
    public string itemName;
    public string itemType;
    public string itemDesc;

    public bool isSpecialItem;
    public int itemQuantityMax = 10;

    //public string itemID;

    public bool Equals(Inventory_ItemSO other)
    {
        if (other == null) return false;
        return other == this;
    }
}