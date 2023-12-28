using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : Skill
{
 
    public Taunt()
    {
        skillName = "Taunt";
        description = "Forces enemies to target this character";
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
        TauntEffect tauntEffect = new TauntEffect(1);
        tauntEffect.ApplyEffect(user);
        user.animator.SetTrigger("BlockTrigger");
        Debug.Log("Taunt applied");
        battleManager.SelectTargetForEnemy();
        user.isAttacking = false;
        user.isAnimationDone = false;
        yield return new WaitUntil(() => user.isAnimationDone == false);
    }
    
}
