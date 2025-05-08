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
        energyGainBonus = 0;
        numberOfAttacksPossible = 3;
        skillPointCost = 1;
        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_TripleSlash");
    }

    protected override int CalculateBaseDamage(Character user)
    {
        return Mathf.CeilToInt(user.attackPower * 0.4f);
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        int baseDamage = CalculateBaseDamage(user);
        user.animator.SetTrigger("TripleSlashTrigger");

        for (int i = 0; i < numberOfAttacksPossible; i++)
        {
            yield return TimingManager.Instance.HandleTimingWindow(user, target, baseDamage, (TimingEventResult result) =>
            {
                HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
                battleManager.CameraShakeMagnitude(result);
                battleManager.ShowTimingResult(result.ToString());
            });
        }

        yield return new WaitUntil(() => user.isAnimationDone);

        user.isAnimationDone = false;
        user.isAttacking = false;
        target.CheckForDeath();
    }
}
