using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerJob
{
    protected SkillManager skillManager => SkillManager.Instance;

    public abstract int BaseAtk { get; }
    public abstract int BaseDef { get; }
    public abstract int BaseMagAtk { get; }
    public abstract int BaseMagDef { get; }
    public abstract int BaseHealth { get; } 
    public abstract int BaseEnergy { get; }

    public abstract List<SkillType> AvailableSkills { get; }

    public abstract Skill GetSkillInstance(SkillType skillType);
}


