using System.Collections;
using UnityEngine;

public class Slash : Skill
{
    private int numberOfAttacksPossible;

    public Slash()
    {
        // Identity
        Type = SkillType.Slash;
        skillName = "Slash";
        description = "Tap at the right time for extra damage";

        // Energy / XP / misc
        energyCost = 0;
        energyGain = 1;
        energyGainBonus = 1;
        skillLevel = 1;
        requiresMovement = true;
        skillExecutionComplete = false;

        // Icon
        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_Slash");

        // Behavior
        numberOfAttacksPossible = 2;

        // NEW: power + speed
        // A “basic” move in your system might be ~50–70 power.
        power = 60;

        // Slash should feel “snappy”
        speedMultiplier = 1.10f;

        // Optional: slight tie-break edge (0 is fine too)
        priority = 0;

        // Optional variance (I’d usually keep this off early if you’re doing timing already)
        useVariance = false;
        // If you do enable it later:
        // varianceMin = 0.90f;
        // varianceMax = 1.00f;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
    user.isAttacking = true;
    user.isAnimationDone = false;

    // Total damage budget from Power system (60 total)
    int totalDamageBudget = CalculateBaseDamage(
        userAttack: user.attackPower,
        targetDefense: target.defense,
        userLevel: user.level
    );

    // ✅ Front-loaded split: 60% first hit, 40% second hit
    int firstHit = Mathf.CeilToInt(totalDamageBudget * 0.60f);
    int secondHit = Mathf.Max(1, totalDamageBudget - firstHit); // ensures exact total

    int[] hits = { firstHit, secondHit };

    user.animator.SetTrigger("Attack1Trigger");

    for (int i = 0; i < hits.Length; i++)
    {
        int perHitDamage = hits[i];

        yield return TimingManager.Instance.HandleTimingWindow(
            user,
            target,
            perHitDamage,
            (TimingEventResult result) =>
            {
                HandleTimingResultForPlayerAttack(user, target, result, perHitDamage);
                battleManager.CameraShakeMagnitude(result);
                battleManager.ShowTimingResult(result.ToString());
            });
    }

    yield return new WaitUntil(() => user.isAnimationDone);

    user.isAnimationDone = false;
    user.isAttacking = false;
    bonusGained = false;

    user.GainEnergy(energyGain);
    target.CheckForDeath();
}
}