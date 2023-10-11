using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EquipmentManager : MonoBehaviour {
    public Equipment[] currentEquipment; // Array to hold currently equipped items
    public Inventory inventory; // Reference to the player's inventory
    public InventoryUI inventoryUI;
    public Slot[] equipmentSlots; // Array to hold the equipment slots UI elements
    
   


    
    // Delegate for equipment change event
    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private void Start() {
        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    public int Equip(Equipment newEquipment) {
        int slotIndex = (int)newEquipment.equipmentType;
        Debug.Log("Equipping: " + newEquipment.name + " to index: " + slotIndex);
  

        Equipment oldEquipment = null;

        // Check if there's already an item equipped in this slot
        if (currentEquipment[slotIndex] != null) {
            oldEquipment = currentEquipment[slotIndex];
            inventory.items.Add(oldEquipment); // Add the old equipment back to the inventory
        }

        // Equip the new item
        currentEquipment[slotIndex] = newEquipment;
        Debug.Log("Equipped to slot index: " + slotIndex);


        // Trigger an equipment change event, if there are any subscribers
        if (onEquipmentChanged != null) {
            onEquipmentChanged.Invoke(newEquipment, oldEquipment);
        }
        inventoryUI.UpdateUI();
        return slotIndex;

    }

    public void Unequip(int slotIndex) 
    {
        Debug.Log("Trying to unequip from index: " + slotIndex);
        if (currentEquipment[slotIndex] != null)
        {
            Equipment itemToUnequip = currentEquipment[slotIndex];
            inventory.items.Add(itemToUnequip);  // Adding it back to the inventory here, ensure this is the only place you're doing it
            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null) 
            {
                onEquipmentChanged.Invoke(null, itemToUnequip);
            }

            inventoryUI.UpdateUI();
        }
    }
    

    // Function to unequip all items
    public void UnequipAll() {
        for (int i = 0; i < currentEquipment.Length; i++) {
            Unequip(i);
        }
    }
}

