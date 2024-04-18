using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    private int numberOfAttacksPossible;
    

    public TripleHitSkill()
    {
        skillName = "Triple Slash";
        description = "Tap before each hit for extra damage";
        requiresMovement = true;
        energyCost = 3;
        numberOfAttacksPossible = 3;
        iconImage = LoadIconImage("SkillIcons/fire2");
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)Math.Ceiling(user.attackPower * 0.7f); //Rounds up to next integer of any fraction
    }
    

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;  // Reset the flag at the start of each attack



        int baseDamage = CalculateBaseDamage(user);
        description = "Slash three times for " + baseDamage + " damage each hit.";
        user.animator.SetTrigger("TripleSlashTrigger");
        for( int i = 0; i < numberOfAttacksPossible; i++)
        {
             

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.7f);
              // yield return battleManager.StartCoroutine(battleManager.TimeStop(0.15f));
             

             HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
             Debug.Log("Timing for player attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             
             yield return new WaitUntil(() => user.animationDamageTime == true);
             
        }
        Debug.Log("waiting on animation to finish");
    
        yield return new WaitUntil(() => user.isAnimationDone == true);
        user.animationDamageTime = false;
        user.isAnimationDone = false;
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