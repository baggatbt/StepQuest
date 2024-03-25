using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectDamagePassive : Skill
{
    public ReflectDamagePassive()
    {
        isActiveSkill = false; // This skill is passive
        // Other initializations...
    }

    
    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        yield break; // No operation for passive skills
    }
}
