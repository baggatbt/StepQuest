using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




 public enum SlotType
    {
        Inventory,
        Equipment
    }



public class Slot : MonoBehaviour       
{
    public SlotType slotType;
    public EquipmentType? equipmentType; // Only used if slotType is Equipment
   
    public Image icon;
    public Button removeButton; //  remove items from inventory
    public EquipmentManager equipmentManager; 
    public InventoryUI inventoryUI;

    public Inventory inventory; 
    public int slotIndex; 

    public Equipment item;

    
    private void Start() 
{
    if (inventory.items.Count > 0) 
    {
        Equipment tempItem = inventory.items[0]; // Get the first item
      //  AddItem(tempItem);
    }
}


    public void EquipFromInventory()
{
    if (item != null) 
    {
        int equipmentSlotIndex = equipmentManager.Equip(item);
        inventoryUI.UpdateEquipmentSlotUI(equipmentSlotIndex, item); // New line
        inventory.items.Remove(item);
        ClearSlot();
    }
}



    // Handle the item click for both inventory and equipment slots
    public void OnItemClicked()
    {
        if (slotType == SlotType.Inventory)
        {
            EquipFromInventory();

        }
        else if (slotType == SlotType.Equipment && item != null)
        {
            UnequipItem();
        }
    }

    private void UnequipItem()
{
    if (item != null)
    {
        inventory.items.Add(item);
        inventoryUI.UpdateInventorySlotUI(item); // New line
        equipmentManager.Unequip((int)equipmentType);
        ClearSlot();
    }
}


    public void AddItem(Equipment newItem)
    {
    item = newItem;
    icon.sprite = item.equipmentIcon;
    Debug.Log(icon.sprite);
    icon.enabled = true;
   // removeButton.interactable = true; 
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        //removeButton.interactable = false;
    }




}



