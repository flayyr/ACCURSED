[System.Serializable]
public class ItemStack
{
    public string itemName;
    public int quantity;

    public ItemStack(string name, int qty)
    {
        itemName = name;
        quantity = qty;
    }
}