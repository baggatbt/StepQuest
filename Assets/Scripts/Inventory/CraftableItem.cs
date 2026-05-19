using UnityEngine;

[CreateAssetMenu(fileName = "New Craftable Item", menuName = "Crafting/Craftable Item")]
public class CraftableItem : Item
{
    [Header("Craft Result")]
    public Item craftedItemResult;
    public Equipment craftedEquipmentResult;

    private void OnValidate()
    {
        itemType = ItemType.CraftableItem;
    }

    public bool CraftsEquipment()
    {
        return craftedEquipmentResult != null;
    }

    public Item GetNormalCraftResult()
    {
        return craftedItemResult;
    }

    public Equipment GetEquipmentCraftResult()
    {
        return craftedEquipmentResult;
    }
}