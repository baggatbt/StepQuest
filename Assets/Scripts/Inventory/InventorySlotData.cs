using System;

[Serializable]
public class InventorySlotData
{
    public int itemID = -1;
    public int quantity = 0;

    public bool IsEmpty => itemID < 0 || quantity <= 0;

    public InventorySlotData()
    {
        Clear();
    }

    public InventorySlotData(int itemID, int quantity)
    {
        this.itemID = itemID;
        this.quantity = quantity;
    }

    public void Set(int newItemID, int newQuantity)
    {
        itemID = newItemID;
        quantity = newQuantity;
    }

    public void Clear()
    {
        itemID = -1;
        quantity = 0;
    }
}