using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/SkillConfig", fileName = "NewSkillConfig")]
public class SkillConfig : ScriptableObject
{
    [Tooltip("Must match MissionDefinition.skillID")]
    public string skillID;

    [Header("Exponential XP Growth")]
    [Tooltip("Base XP needed at level 1.")]
    public float baseXP = 10f;

    [Tooltip("Multiply XP by this^level (e.g. 1.1 = 10% more each level)")]
    public float growthRate = 1.1f;

    [Tooltip("Flat bonus added after exponential calculation")]
    public float additive = 0f;

    [Tooltip("Maximum achievable level")]
    [Min(1)]
    public int maxLevel = 100;
}
