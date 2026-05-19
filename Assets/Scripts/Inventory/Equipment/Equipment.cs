using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    [Header("Equipment")]
    public EquipmentType equipmentType;

    [Header("Rolled Stats")]
    public int attackBonus;
    public int defenseBonus;
    public int maxHealthBonus;
    public int maxEnergyBonus;
    public int speedBonus;

    [Header("Roll Setup")]
    public List<EquipmentStatRange> guaranteedStatRanges = new List<EquipmentStatRange>();
    public List<SecretEquipmentStatRange> secretStatRanges = new List<SecretEquipmentStatRange>();

    [Header("Instance Data")]
    public bool hasBeenRolled;
    public string uniqueInstanceId;

    private void OnValidate()
    {
        itemType = ItemType.Equipment;
    }

    public override bool IsStackable()
    {
        return false;
    }

    public void RollNewStats()
    {
        uniqueInstanceId = Guid.NewGuid().ToString();
        hasBeenRolled = true;

        attackBonus = 0;
        defenseBonus = 0;
        maxHealthBonus = 0;
        maxEnergyBonus = 0;
        speedBonus = 0;

        foreach (EquipmentStatRange range in guaranteedStatRanges)
        {
            int rolledValue = UnityEngine.Random.Range(range.minValue, range.maxValue + 1);
            AddStat(range.statType, rolledValue);
        }

        foreach (SecretEquipmentStatRange secretRange in secretStatRanges)
        {
            float roll = UnityEngine.Random.Range(0f, 100f);

            if (roll <= secretRange.chancePercent)
            {
                int rolledValue = UnityEngine.Random.Range(secretRange.minValue, secretRange.maxValue + 1);
                AddStat(secretRange.statType, rolledValue);
                Debug.Log($"Secret stat rolled on {itemName}: +{rolledValue} {secretRange.statType}");
            }
        }

        quantity = 1;
    }

    public void AddStat(EquipmentStatType statType, int amount)
    {
        switch (statType)
        {
            case EquipmentStatType.Attack:
                attackBonus += amount;
                break;

            case EquipmentStatType.Defense:
                defenseBonus += amount;
                break;

            case EquipmentStatType.MaxHealth:
                maxHealthBonus += amount;
                break;

            case EquipmentStatType.MaxEnergy:
                maxEnergyBonus += amount;
                break;

            case EquipmentStatType.Speed:
                speedBonus += amount;
                break;
        }
    }

    public string GetStatDescription()
    {
        List<string> lines = new List<string>();

        if (attackBonus != 0) lines.Add($"+{attackBonus} ATK");
        if (defenseBonus != 0) lines.Add($"+{defenseBonus} DEF");
        if (maxHealthBonus != 0) lines.Add($"+{maxHealthBonus} HP");
        if (maxEnergyBonus != 0) lines.Add($"+{maxEnergyBonus} Energy");
        if (speedBonus != 0) lines.Add($"+{speedBonus} SPD");

        if (lines.Count == 0)
            return "No bonus stats.";

        return string.Join("\n", lines);
    }

    public void LoadRolledData(
        string savedInstanceId,
        int savedAttack,
        int savedDefense,
        int savedMaxHealth,
        int savedMaxEnergy,
        int savedSpeed)
    {
        uniqueInstanceId = savedInstanceId;
        hasBeenRolled = true;

        attackBonus = savedAttack;
        defenseBonus = savedDefense;
        maxHealthBonus = savedMaxHealth;
        maxEnergyBonus = savedMaxEnergy;
        speedBonus = savedSpeed;

        quantity = 1;
    }
}

public enum EquipmentType
{
    Helm,
    Chest,
    Hand,
    Foot
}

public enum EquipmentStatType
{
    Attack,
    Defense,
    MaxHealth,
    MaxEnergy,
    Speed
}

[Serializable]
public class EquipmentStatRange
{
    public EquipmentStatType statType;
    public int minValue;
    public int maxValue;
}

[Serializable]
public class SecretEquipmentStatRange
{
    public EquipmentStatType statType;
    public int minValue;
    public int maxValue;

    [Range(0f, 100f)]
    public float chancePercent = 5f;
}