using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryUI : MonoBehaviour 
{
    public Inventory inventory;
    public Transform itemsParent; // The parent object holding all the inventory slots
    private List<Slot> inventorySlots;
    private List<Slot> equipmentSlots;

    private void Start() 
    {
        // Separate out inventory and equipment slots
        inventorySlots = itemsParent.GetComponentsInChildren<Slot>().Where(slot => slot.slotType == SlotType.Inventory).ToList();
        equipmentSlots = itemsParent.GetComponentsInChildren<Slot>().Where(slot => slot.slotType == SlotType.Equipment).ToList();

        Debug.Log("Inventory Slots count: " + inventorySlots.Count);
        Debug.Log("Equipment Slots count: " + equipmentSlots.Count);
        Debug.Log("Inventory reference: " + inventory);

        UpdateUI();
    }

    public void UpdateUI()
    {
        Debug.Log("UpdateUI being called");

        // Handle Inventory Slots
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < inventory.items.Count)
            {
                inventorySlots[i].AddItem(inventory.items[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }

        // Handle Equipment Slots
        // For now, just clear them. 
        foreach (Slot slot in equipmentSlots)
        {
            slot.ClearSlot();
        }
    }

    public void UpdateEquipmentSlotUI(int slotIndex, Equipment item)
{
    if (slotIndex < equipmentSlots.Count)
    {
        equipmentSlots[slotIndex].AddItem(item);
    }
}

public void UpdateInventorySlotUI(Equipment item)
{
    // Find the first empty slot
    for (int i = 0; i < inventorySlots.Count; i++)
    {
        if (inventorySlots[i].item == null)
        {
            inventorySlots[i].AddItem(item);
            break;
        }
    }
}

    
}
