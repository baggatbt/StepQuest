using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct AttackStage
{
    public string animationTrigger;
    public float timingWindowStart;
    public float timingWindowEnd;
    public int damage;
}

public enum SkillType
    {
        None,
        Slash,
        TripleHit,
        SwordWave,
    }



public abstract class Skill
{
    public string skillName;
    public string description;
    public bool skillExecutionComplete; // Flag to track the completion of skill execution
    public bool requiresMovement; // Indicates if the attack requires movement towards the target
    public int energyCost;
    public int skillLevel;

    

    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);

    public int GetSkillLevel()
    {
        // Check if the player's data contains a level for this skill
        if (PlayerData.Instance.skillLevels.TryGetValue(skillName, out int level))
        {
            // If it does, return that level
            return level;
        }
        else
        {
            // If it doesn't, return a default level 
            return 1;
        }
    }
}
