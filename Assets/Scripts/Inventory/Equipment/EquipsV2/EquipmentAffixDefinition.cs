using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Equipment/Affix Definition")]
public class EquipmentAffixDefinition : ScriptableObject
{
    [Header("Display")]
    public string affixName;

    [TextArea]
    public string description;

    [Header("Stat Bonus")]
    public EquipmentStatType statType;
    public int minValue = 1;
    public int maxValue = 3;

    [Header("Roll Weight")]
    [Tooltip("Higher means more common.")]
    public int weight = 10;

    public int RollValue()
    {
        return Random.Range(minValue, maxValue + 1);
    }
}