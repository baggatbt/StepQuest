using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerJob
{
    private int playerLevel => PlayerData.Instance.level;

    public override int BaseAtk => 10; 
    public override int BaseDef => 1; 
    public override int BaseMagAtk => 2; 
    public override int BaseMagDef => 2; 
    public override int BaseHealth => 10; 
    
   

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
