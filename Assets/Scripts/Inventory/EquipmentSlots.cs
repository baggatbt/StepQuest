using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EquipmentSlots
{
    public Equipment Helm;
    public Equipment Chest;
    public Equipment Hand;
    public Equipment Foot;

    public Equipment GetEquipment(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Helm: return Helm;
            case EquipmentType.Chest: return Chest;
            case EquipmentType.Hand: return Hand;
            case EquipmentType.Foot: return Foot;
            default: return null;
        }
    }

    public void SetEquipment(EquipmentType type, Equipment equipment)
    {
        switch (type)
        {
            case EquipmentType.Helm: Helm = equipment; break;
            case EquipmentType.Chest: Chest = equipment; break;
            case EquipmentType.Hand: Hand = equipment; break;
            case EquipmentType.Foot: Foot = equipment; break;
        }
    }
}
