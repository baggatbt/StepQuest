using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Companion : Character
{
    public abstract List<SkillType> AvailableSkills { get; }
    public abstract List<SkillType> LockedSkills { get; }
    public abstract Skill GetSkillInstance(SkillType skillType);
    
    // Additional companion-specific properties and behavior
}

   


