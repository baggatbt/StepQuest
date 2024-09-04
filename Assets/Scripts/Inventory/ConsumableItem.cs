using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConsumableItem", menuName = "Inventory/Consumable Item")]
public class ConsumableItem : Item
{
     public int healthRecoveryAmount;  // How much health this item recovers
   
    

    
    // Define any other effects or properties specific to consumables here.
    
    public void Consume(Companion companion)
    {
        // Check if the consumable is indeed a health potion
        if (itemType == ItemType.Consumable)
        {
            // Apply the consumable's effect, e.g., recovering health.
            companion.RecoverHealth(healthRecoveryAmount);
            
            // reduce the quantity of the consumable in the inventory
            quantity--;
        }

        // If quantity falls below 1, it could be removed from the inventory.
        if (quantity < 1)
        {
            GameManager.Instance.RemoveItem(this, 0);
        }
    }
}

