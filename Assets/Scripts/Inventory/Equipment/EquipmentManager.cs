using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Equipment[] equippedItems; // Array of equipped items
    public List<Equipment> inventory = new List<Equipment>(); // List of items in inventory

    private void Awake()
    {
        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        equippedItems = new Equipment[numSlots];
    }

    public void Equip(Equipment item)
    {
        int slotIndex = (int)item.equipmentType;
        if (equippedItems[slotIndex] == null && inventory.Contains(item))
        {
            equippedItems[slotIndex] = item;
            inventory.Remove(item);
        }
    }

    public void Unequip(EquipmentType type)
    {
        int slotIndex = (int)type;
        Equipment itemToUnequip = equippedItems[slotIndex];
        if (itemToUnequip != null)
        {
            inventory.Add(itemToUnequip);
            equippedItems[slotIndex] = null;
        }
    }

    public Equipment GetEquippedItem(EquipmentType type)
    {
        return equippedItems[(int)type];
    }
}
