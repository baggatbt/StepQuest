using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerJob
{
    

    public abstract int BaseAtk { get; }
    public abstract int BaseDef { get; }
    public abstract int BaseMagAtk { get; }
    public abstract int BaseMagDef { get; }
    public abstract int BaseHealth { get; } 
   
    public  int JobExp;
    public  int JobLevel;
    public  int JobPoints;

    public abstract List<SkillType> AvailableSkills { get; }

    public abstract List<SkillType> LockedSkills { get; }

    public abstract Skill GetSkillInstance(SkillType skillType);
}


