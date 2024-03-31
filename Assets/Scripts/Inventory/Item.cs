using UnityEngine;

public enum ItemType
{
    Equipment,
    Consumable,
    Quest
    // Add other types as needed
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemID;
    public string itemDescription;
}
