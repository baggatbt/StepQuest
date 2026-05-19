using System.Collections;
using UnityEngine;

public class Slash : Skill
{
    private int numberOfAttacksPossible;

    public Slash()
    {
        Type = SkillType.Slash;
        skillName = "Slash";
        description = "Tap at the right time for extra damage.";

        energyCost = 0;
        energyGain = 1;
        energyGainBonus = 1;
        skillLevel = 1;
        requiresMovement = true;
        skillExecutionComplete = false;

        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_Slash");

        numberOfAttacksPossible = 2;

        damageMultiplier = 1.0f;
        damageVarianceMin = 1.0f;
        damageVarianceMax = 1.2f;

        speedMultiplier = 1.10f;
        priority = 0;

        useVariance = false;
    }

    public override Vector2Int GetBaseDamageRange(Character user)
    {
        float originalMultiplier = damageMultiplier;

        KnightSlashModifier modifier = GetKnightSlashModifier(user);

        switch (modifier)
        {
            case KnightSlashModifier.HeavySlash:
                damageMultiplier *= 1.25f;
                break;

            case KnightSlashModifier.QuickSlash:
                damageMultiplier *= 0.85f;
                break;
        }

        Vector2Int range = base.GetBaseDamageRange(user);

        damageMultiplier = originalMultiplier;

        return range;
    }

    public override int GetEffectiveSpeed(int userSpeed)
    {
        // This method does not receive the Character user, so it can only use the normal speed.
        // The actual Knight modifier preview is handled in GetBattlePreviewText().
        return base.GetEffectiveSpeed(userSpeed);
    }

    public override string GetBattlePreviewText(Character user)
    {
        Vector2Int dmgRange = GetBaseDamageRange(user);
        int finalSpeed = GetModifiedSpeed(user);

        string modifierName = GetKnightSlashModifier(user).ToString();

        if (GetKnightSlashModifier(user) == KnightSlashModifier.None)
            return $"MP: {energyCost}   SPD: {finalSpeed}   DMG: {dmgRange.x}-{dmgRange.y}";

        return $"{modifierName}\nMP: {energyCost}   SPD: {finalSpeed}   DMG: {dmgRange.x}-{dmgRange.y}";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        KnightSlashModifier modifier = GetKnightSlashModifier(user);

        int totalDamageBudget = RollBaseDamage(user);

        int firstHit = Mathf.CeilToInt(totalDamageBudget * 0.60f);
        int secondHit = Mathf.Max(1, totalDamageBudget - firstHit);

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

                    if (modifier == KnightSlashModifier.EnergizingSlash && result == TimingEventResult.Good)
                    {
                        user.GainEnergy(1);
                        Debug.Log("[Slash] Energizing Slash bonus energy gained.");
                    }
                });
        }

        yield return new WaitUntil(() => user.isAnimationDone);

        user.isAnimationDone = false;
        user.isAttacking = false;
        bonusGained = false;

        user.GainEnergy(energyGain);
        target.CheckForDeath();
    }

    private KnightSlashModifier GetKnightSlashModifier(Character user)
    {
        if (user is Companion companion && companion.characterData != null)
            return companion.characterData.knightSlashModifier;

        return KnightSlashModifier.None;
    }

    private int GetModifiedSpeed(Character user)
    {
        if (user == null)
            return 0;

        KnightSlashModifier modifier = GetKnightSlashModifier(user);

        float speedMod = speedMultiplier;

        switch (modifier)
        {
            case KnightSlashModifier.HeavySlash:
                speedMod *= 0.80f;
                break;

            case KnightSlashModifier.QuickSlash:
                speedMod *= 1.25f;
                break;
        }

        return Mathf.Max(1, Mathf.RoundToInt(user.speed * speedMod));
    }
}