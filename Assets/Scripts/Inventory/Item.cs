using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    Equipment,
    Consumable,
    Quest,
    MaterialItem,
    CraftableItem,

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
    public int quantity = 1; // Default quantity
    public int stepCostToProduce;
    public List<MaterialRequirement> materialRequirements;
}

[CreateAssetMenu(fileName = "New Craftable Item", menuName = "Inventory/Craftable Item")]
public class CraftableItem : Item
{
   // public List<MaterialRequirement> materialRequirements;
    // Other properties specific to the craftable item...
}

[System.Serializable]
public class MaterialRequirement
{
    public MaterialItem material;
    public int quantity;
}

