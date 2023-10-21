using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSkill : Skill
{
 
    public GuardSkill()
    {
        skillName = "Guard";
        description = "Increases defense by 50% for two turns";
        energyCost = 2; 
        energyGain = 0;
        skillLevel = 1; 
        requiresMovement = false; 
        skillExecutionComplete = false;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;
        DefenseUp defenseUpEffect = new DefenseUp(2);
        battleManager.statusEffectController.AddEffect(defenseUpEffect, user, 2);
        Debug.Log("Guard skill applied");
        user.isAttacking = false;
        user.animator.SetTrigger("GuardTrigger");
        yield return new WaitUntil(() => user.isAnimationDone == true);
        
    }
    
}
