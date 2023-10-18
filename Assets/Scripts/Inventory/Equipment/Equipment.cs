using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentType equipmentType;
    public int attackBonus;
    public int defenseBonus;
   
}


public enum EquipmentType { Helm, Chest, Hand, Foot }
