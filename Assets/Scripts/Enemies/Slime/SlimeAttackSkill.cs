using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    public SlimeAttackSkill()
    {
        skillName = "Slime Pounce";
        description = "Pounces onto the target. Damage can be reduced by a timely action.";
        requiresMovement = false; // we handle motion ourselves
    }

    protected override int CalculateBaseDamage(Character user)
    {
        // tweak as desired
        return Mathf.RoundToInt(user.attackPower * 1.0f);
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        int baseDamage = CalculateBaseDamage(user);

        var motionBinder = user.GetComponent<SlimePounceMotion>();
        var mover        = user.GetComponent<AttackMotionController>();

        if (motionBinder != null) motionBinder.target = target.transform;

        // Play your slime's pounce animation (in-place hop frames)
        // Either a trigger or direct state name works; match your Animator
        //user.animator.ResetTrigger("GoblinAttack1Trigger"); // if you reuse anim params, clear them
        user.animator.SetTrigger("SlimeAttack1Trigger");     // <-- create this trigger in Animator
        // Alternatively: user.animator.Play("Pounce");

        // Wait exactly until the landing/impact event fires
        if (motionBinder != null)
            yield return motionBinder.WaitForLanding();

        // Timing window + damage (same flow you already use)
        yield return TimingManager.Instance.HandleTimingWindow(
            user, target, baseDamage,
            (TimingEventResult result) =>
            {
                HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
                battleManager.CameraShakeMagnitude(result);
            });

        // Let the animation finish recovery; or gently return visual to base
        if (mover != null)
            yield return mover.ReturnToBase(0.12f);

        // Wait for anim end if you set user.isAnimationDone in an event
        yield return new WaitUntil(() => user.isAnimationDone);

        user.isAnimationDone = false;
        user.isAttacking = false;
        target.CheckForDeath();
    }
}
