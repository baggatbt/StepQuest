using UnityEngine;

[System.Serializable]
public class GuildObjective
{
    public string objectiveName;

    [TextArea]
    public string description;

    public string requiredEnemyID;
    public int requiredKills;
    public int currentKills;

    public int copperReward;
    public int expReward;

    public bool isComplete;

    public string ProgressText()
    {
        return $"{currentKills}/{requiredKills}";
    }
}