using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    private int numberOfAttacksPossible;

    public GoblinAttackSkill()
    {
        skillName = "Goblin Attack";
        description = "The Goblin attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        numberOfAttacksPossible = 1;
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.0f);
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        int baseDamage = CalculateBaseDamage(user);

        // NEW: hand the motion script the current target so BeginLunge knows direction
       // var motionBinder = user.GetComponent<GoblinAttackMotion>();     // NEW
       // var motionCtrl   = user.GetComponent<AttackMotionController>();  // NEW
      //  if (motionBinder != null) motionBinder.target = target.transform; // NEW

        // Fire the normal goblin attack animation
        user.animator.SetTrigger("GoblinAttack1Trigger");

        for (int i = 0; i < numberOfAttacksPossible; i++)
        {
            // Your existing timing/damage flow
            yield return TimingManager.Instance.HandleTimingWindow(
                user, target, baseDamage,
                (TimingEventResult result) =>
                {
                    HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
                    battleManager.CameraShakeMagnitude(result);
                });
        }

        // Wait for the animation to finish (unchanged)
        yield return new WaitUntil(() => user.isAnimationDone);

        // Safety: ensure visual returns to idle anchor even if an event was missed
       // if (motionCtrl != null) motionCtrl.ResetVisual(); // NEW

        // Reset flags (unchanged)
        user.isAnimationDone = false;
        user.isAttacking = false;
        target.CheckForDeath();
    }
}
