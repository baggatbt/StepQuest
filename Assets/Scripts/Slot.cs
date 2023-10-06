using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
    {
    public Image icon;
    public Button removeButton; //  remove items from inventory
    public EquipmentManager equipmentManager; 
    public InventoryUI inventoryUI;

    public Inventory inventory; 
    public int slotIndex; 

    private Equipment item;

    
    public void OnEquipButton()
     {
        if (item != null) {
            equipmentManager.Equip(item);
            //remove the item from the inventory
            inventory.items.Remove(item);
            ClearSlot();
            // Update the inventory UI again
            inventoryUI.UpdateUI(slotIndex);

        }

    
    }

    public void AddItem(Equipment newItem) {
    item = newItem;
    icon.sprite = item.equipmentIcon;
    icon.enabled = true;
   // removeButton.interactable = true; 
}

    public void ClearSlot() {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        //removeButton.interactable = false;
    }


}



