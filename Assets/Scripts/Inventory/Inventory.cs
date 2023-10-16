using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory : MonoBehaviour
{
    public List<Equipment> items = new List<Equipment>();
    public int maxItems = 20;
    public EquipmentSlots equippedItems = new EquipmentSlots();

    public void Add(Equipment item)
    {
        if (items.Count < maxItems) items.Add(item);
    }

    public void Remove(Equipment item)
    {
        items.Remove(item);
    }

    public void Equip(Equipment item)
    {
        // Unequip the current item if there's one in the slot
        Equipment currentItem = equippedItems.GetEquipment(item.equipmentType);
        if(currentItem != null)
        {
            Unequip(item.equipmentType);
        }

        // Equip the new item
        equippedItems.SetEquipment(item.equipmentType, item);
        Remove(item);
    }

    public void Unequip(EquipmentType type)
    {
        Equipment currentItem = equippedItems.GetEquipment(type);
        if(currentItem != null)
        {
            Add(currentItem);
            equippedItems.SetEquipment(type, null);
        }
    }
}
