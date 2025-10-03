using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    private int numberOfAttacksPossible;

    public SlimeAttackSkill()
    {
        skillName = "Slime Attack";
        description = "The slime attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        numberOfAttacksPossible = 1;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.0f); 
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;  

        int baseDamage = CalculateBaseDamage(user);

        user.animator.SetTrigger("Attack1Trigger");

        for (int i = 0; i < numberOfAttacksPossible; i++)
    {
       

        // Use TimingManager to handle the timing and damage
        yield return TimingManager.Instance.HandleTimingWindow(user, target, baseDamage, (TimingEventResult result) =>
        {
            // Call the centralized damage handling method
            HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
            battleManager.CameraShakeMagnitude(result);
            
        });

        
    }

      // Wait for the animation to finish
    yield return new WaitUntil(() => user.isAnimationDone);
    

    // Reset flags
    user.isAnimationDone = false;
    user.isAttacking = false;
    target.CheckForDeath();
}
}
