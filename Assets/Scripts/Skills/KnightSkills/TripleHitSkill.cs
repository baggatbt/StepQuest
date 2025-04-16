using System.Collections;
using UnityEngine;
using System;

public class TripleHitSkill : Skill
{
     private int numberOfAttacksPossible;
    public TripleHitSkill()
    {
        skillName = "Triple Slash";
        description = "Tap before each hit for extra damage";
        requiresMovement = true;
        energyCost = 3;
        energyGain = 0;
        numberOfAttacksPossible = 3;
        skillPointCost = 1;
        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_TripleSlash");
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return (int)Math.Ceiling(user.attackPower * 0.7f); // Rounds up to next integer of any fraction
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
   // TimingVisualAid visualAid = battleManager.visualAid; // Get the visual aid from the BattleManager

    user.isAttacking = true;
    user.isAnimationDone = false; // Reset the flag at the start of the attack

    int baseDamage = CalculateBaseDamage(user);
    user.animator.SetTrigger("TripleSlashTrigger");

    for (int i = 0; i < numberOfAttacksPossible; i++)
    {
       

        // Use TimingManager to handle the timing and damage
        yield return TimingManager.Instance.HandleTimingWindow(user, target, baseDamage, (TimingEventResult result) =>
        {
            // Call the centralized damage handling method
            HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
            battleManager.CameraShakeMagnitude(result);
            battleManager.ShowTimingResult(result.ToString());
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