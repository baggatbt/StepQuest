using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    Equipment,
    Consumable,
    Quest,
    MaterialItem,
    CraftableItem,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemID;
    [TextArea] public string itemDescription;

    public int quantity = 1;

    public int stepCostToProduce;
    public List<MaterialRequirement> materialRequirements;

    public virtual bool IsStackable()
    {
        return itemType != ItemType.Equipment;
    }
}

[System.Serializable]
public class MaterialRequirement
{
    public MaterialItem material;
    public int quantity;
}