using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerJob
{
    private int playerLevel => PlayerData.Instance.level;

    public override int BaseAtk => 8; 
    public override int BaseDef => 3; 
    public override int BaseMagAtk => 5; 
    public override int BaseMagDef => 2; 
    public override int BaseHealth => 27; 
    public override int BaseEnergy => 10;
   

    public override List<SkillType> AvailableSkills => new List<SkillType>
    {
        SkillType.TripleHit,
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
