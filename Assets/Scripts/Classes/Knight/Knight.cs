using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerJob
{
    private int playerLevel => PlayerData.Instance.level;

    public override int BaseAtk => 8 + (playerLevel * 2);
    public override int BaseDef => 3 + playerLevel;  
    public override int BaseMagAtk => 5 + (playerLevel);  
    public override int BaseMagDef => 2 + playerLevel;  
    public override int BaseHealth => 27 + (playerLevel * 3);
    public override int BaseEnergy => 10;
    public override int JobExp => 0; //Need to implement function to make this go up, and move skills to available

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.GuardSkill,
        SkillType.SwordWave,
        
    };

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        
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

            case SkillType.GuardSkill:
                return new GuardSkill();

            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
}
