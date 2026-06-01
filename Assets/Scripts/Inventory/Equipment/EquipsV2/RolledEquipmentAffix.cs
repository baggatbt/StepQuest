using System;

[Serializable]
public class RolledEquipmentAffix
{
    public string affixName;
    public EquipmentStatType statType;
    public int value;

    public RolledEquipmentAffix(string affixName, EquipmentStatType statType, int value)
    {
        this.affixName = affixName;
        this.statType = statType;
        this.value = value;
    }

    public string GetDisplayText()
    {
        string statName = statType.ToString();
        return $"+{value} {statName}";
    }
}