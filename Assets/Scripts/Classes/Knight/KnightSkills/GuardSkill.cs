using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSkill : Skill
{
 
    public GuardSkill()
    {
        skillName = "Guard";
        description = "Increase DEF by 50% for two turns.";
        requiresMovement = false;
        energyCost = 2;
        skillLevel = 1; 
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        DefenseUp defenseUpEffect = new DefenseUp(2);
        battleManager.statusEffectController.AddEffect(defenseUpEffect, user, 2);
        Debug.Log("Guard skill applied");
        user.isAttacking = false;
        yield return null;
    }
}
