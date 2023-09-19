using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWave : Skill
{
    private SkillManager skillManager;
    

    public SwordWave( SkillManager manager)
    {
        skillManager = manager;
        skillName = "SwordWave";
        description = "Release a powerful wave from your sword.";
        requiresMovement = false;
        energyCost = 2;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.5);  // 150% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        int baseDamage = CalculateBaseDamage(user);

        yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 1.0f, (result) =>
        {
             HandleTimingResultForPlayerAttack(user, target, "Attack1Trigger", result, baseDamage);
             skillExecutionComplete = true;
             user.isAttacking = false;
        
        });
     }
}
       
      
        





