/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerJob
{
    private const int AttackPowerLevel69 = 604; // Attack power at level 69
    private const int LevelTransition = 70; // Level where growth rate changes with the goal of 1000 atk by max level.

    private int playerLevel => PlayerData.Instance.level;

    public override int BaseAtk => 5;
    public override int BaseDef => 4;
    public override int BaseMagAtk => 2;
    public override int BaseMagDef => 3;
    public override int BaseHealth => 20;

    // Other methods and properties...

   

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
*/