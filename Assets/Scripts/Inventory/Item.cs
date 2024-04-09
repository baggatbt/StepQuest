using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    Equipment,
    Consumable,
    Quest,
    MaterialItem,

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
}

[CreateAssetMenu(fileName = "New Craftable Item", menuName = "Inventory/Craftable Item")]
public class CraftableItem : Item
{
    public List<MaterialRequirement> materialRequirements;
    // Other properties specific to the craftable item...
}

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public int healthRecoveryAmount;
    public int staminaRecoveryAmount;

    // Define any other effects or properties specific to consumables here.
    
    public void Consume(Companion companion)
    {
        // Apply the consumable's effect, e.g., recovering health.
        // companion.RecoverHealth(healthRecoveryAmount);
        
        // Optionally, handle the removal of the consumable from the inventory here.
    }
}



[System.Serializable]
public class MaterialRequirement
{
    public MaterialItem material;
    public int quantity;
}

