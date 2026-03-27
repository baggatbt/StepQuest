[System.Serializable]
public class InventoryStack
{
    public int itemID;
    public int quantity;

    public InventoryStack(int itemID, int quantity)
    {
        this.itemID = itemID;
        this.quantity = quantity;
    }
}