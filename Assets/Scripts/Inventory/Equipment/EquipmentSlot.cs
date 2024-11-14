using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot
{
    public EquipmentType slotType;
    public Equipment equippedItem;

    public EquipmentSlot(EquipmentType type)
    {
        slotType = type;
    }

    public void Equip(Equipment item)
    {
        equippedItem = item;
    }

    public void Unequip()
    {
        equippedItem = null;
    }
}
