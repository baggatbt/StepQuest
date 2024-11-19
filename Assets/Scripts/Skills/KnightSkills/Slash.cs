using System.Collections;
using UnityEngine;
using System;

public class Slash : Skill
{
    private int numberOfAttacksPossible;
   // GameObject particleEffectPrefab = Resources.Load<GameObject>("PreFab/Projectiles_Effects/skillAttack2");
    public Slash()
    {
        skillName = "Slash";
        description = "Tap at the right time for extra damage";
        energyCost = 0; 
        energyGain = 1;
        skillLevel = 1; 
        requiresMovement = true; 
        skillExecutionComplete = false;
        numberOfAttacksPossible = 2;
        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_Slash");
        
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return (int)Math.Ceiling(user.attackPower * 0.6f); // Rounds up to next integer of any fraction
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
   // TimingVisualAid visualAid = battleManager.visualAid; // Get the visual aid from the BattleManager

    user.isAttacking = true;
    user.isAnimationDone = false; // Reset the flag at the start of the attack

    int baseDamage = CalculateBaseDamage(user);
    user.animator.SetTrigger("Attack1Trigger");

    for (int i = 0; i < numberOfAttacksPossible; i++)
    {
       

        // Use TimingManager to handle the timing and damage
        yield return TimingManager.Instance.HandleTimingWindow(user, target, baseDamage, (TimingEventResult result) =>
        {
            // Call the centralized damage handling method
            HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
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