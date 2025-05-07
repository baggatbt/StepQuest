using System.Collections;
using UnityEngine;
using System;

public class Slash : Skill
{
    private int numberOfAttacksPossible;

    public Slash()
    {
        skillName = "Slash";
        description = "Tap at the right time for extra damage";
        energyCost = 0;
        energyGain = 1;
        energyGainBonus = 1;
        skillLevel = 1;
        requiresMovement = true;
        skillExecutionComplete = false;
        numberOfAttacksPossible = 2;
        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_Slash");
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
        user.animator.SetTrigger("Attack1Trigger");

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
        bonusGained = false;
        user.GainEnergy(energyGain);
        target.CheckForDeath();
    }
}
