using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    private int numberOfAttacksPossible;

    public TripleHitSkill()
    {
        skillName = "Triple Slash";
        description = "Slash up to three times with good timing";
        requiresMovement = true;
        energyCost = 0;
        numberOfAttacksPossible = 4;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 0.5f);  // 70% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        int baseDamage = CalculateBaseDamage(user);
        
        for( int i = 1; i < numberOfAttacksPossible; i++)
        {
             user.isAnimationDone = false;

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.7f);

             HandleTimingResultForPlayerAttack(user, target, "Attack"+i+"Trigger", result, baseDamage);

             yield return new WaitUntil(() => user.isAnimationDone);

             
        }
        
        user.isAttacking = false;
        target.CheckForDeath();
    }


    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
       
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));
         
    }


}