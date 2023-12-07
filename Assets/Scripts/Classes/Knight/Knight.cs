using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerJob
{
    private const int AttackPowerLevel69 = 604; // Attack power at level 69
    private const int LevelTransition = 70; // Level where growth rate changes with the goal of 1000 atk by max level.

    private int playerLevel => PlayerData.Instance.level;

    public override int BaseAtk => CalculateAttackPower(playerLevel);
    public override int BaseDef => 1;
    public override int BaseMagAtk => 2;
    public override int BaseMagDef => 2;
    public override int BaseHealth => 10;

    // Other methods and properties...

    private int CalculateAttackPower(int level)
    {
        if (level < LevelTransition)
        {
            // Calculation for levels 1-69
            return 10 + (int)(4.5f * (level - 1));
        }
        else
        {
            // Calculation for levels 70-100
            return AttackPowerLevel69 + (int)(6.7f * (level - LevelTransition));
        }
       //At Level 69: The attack power would be approximately 316.
       //At Level 100: The attack power would be approximately 523.7.
    }
    
   

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
       
        
    };

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        SkillType.ShieldSlam,
        SkillType.SwordWave
    };

    public override Skill GetSkillInstance(SkillType skillType)
    {
        switch(skillType)
        {
            case SkillType.Slash:
                return new Slash();

            case SkillType.TripleHit:
                return new TripleHitSkill();

            case SkillType.SwordWave:
                return new SwordWave();

            case SkillType.ShieldSlam:
                return new ShieldSlam();
   

            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
}
