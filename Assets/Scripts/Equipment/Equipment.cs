using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment: ScriptableObject {
    public EquipmentType equipmentType;
    public string equipmentName;
    public Sprite equipmentIcon;
    public int attackBonus;
    public int defenseBonus;
    // Add other attributes as needed
}


public enum EquipmentType {
    Helm,
    Chest,
    Foot,
    Hand
}

