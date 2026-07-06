using UnityEngine;

public abstract class ItemPickup : MonoBehaviour
{
    // Inherited by NormalItemPickup and SpecialItemPickup
    public abstract void AddItem(ItemPickupSO item);
    public abstract void ConfirmItem();

}
