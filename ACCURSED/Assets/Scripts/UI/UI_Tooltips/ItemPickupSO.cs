using UnityEngine;

public enum ItemCategory
{
    Consumable,
    Material,
    KeyItem,
    Equipment,
    Misc
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item Pickup")]
public class ItemPickupSO : ScriptableObject
{
    [Header("Identity")]
    public string itemID;
    public GameObject itemShowcaseUIObj;

    [Header("Display")]
    public string itemName;
    public Sprite itemSpr;

    [TextArea(2, 5)] public string itemDesc;

    [Header("Pickup")]
    [Min(1)] public int itemQuantity = 1;
    public bool isSpecialItem;

    [Header("Inventory")]
    public ItemCategory itemCategory = ItemCategory.Misc;

    public bool canUse;

    [Min(1)] public int maxStackSize = 10;
}

// OLD VERSION BELOW

/*
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPickupSO", menuName = "Scriptable Objects/ItemPickupSO")]
public class ItemPickupSO : ScriptableObject
{
    public Sprite itemSpr;
    public string itemName;
    public string itemDesc;
    public bool isSpecialItem;
    public int itemQuantity = 1; // WIP: to be implemented to be assigned from TooltipManager, not manually
    public string itemID;

    //public GameObject promptUIObj;
    public GameObject itemShowcaseUIObj;
}
*/