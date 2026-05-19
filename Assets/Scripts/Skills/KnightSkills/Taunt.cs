using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : Skill
{
    private static readonly Dictionary<Character, int> activeDefenseBuffs = new Dictionary<Character, int>();

    private const int DEFENSE_BONUS = 2;

    public Taunt()
    {
        Type = SkillType.Taunt;
        skillName = "Taunt";
        description = "Draw enemy attention and gain a defense bonus.";

        energyCost = 0;
        energyGain = 1;
        energyGainBonus = 0;

        requiresMovement = false;
        noZoom = true;
        skillExecutionComplete = false;

        damageMultiplier = 0f;
        speedMultiplier = 1.0f;
        priority = 1;

        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_Taunt");
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        ApplyDefenseBuff(user);
        ApplyAggro(user, battleManager);

        if (user.animator != null)
            user.animator.SetTrigger("TauntTrigger");

        yield return new WaitForSeconds(0.5f);

        user.GainEnergy(energyGain);

        user.isAnimationDone = false;
        user.isAttacking = false;

        Debug.Log($"[Taunt] {user.name} used Taunt. +{DEFENSE_BONUS} DEF and increased aggro.");
    }

    private void ApplyDefenseBuff(Character user)
    {
        if (user == null)
            return;

        if (activeDefenseBuffs.ContainsKey(user))
        {
            Debug.Log("[Taunt] Defense buff is already active. Not stacking.");
            return;
        }

        user.defensePower += DEFENSE_BONUS;
        activeDefenseBuffs[user] = DEFENSE_BONUS;
    }

    private void ApplyAggro(Character user, BattleManager battleManager)
    {
        if (GameManager.Instance == null || battleManager == null)
            return;

        if (battleManager.companion1 == user)
        {
            GameManager.Instance.probabilityCompanion1 = 10f;
            GameManager.Instance.probabilityCompanion2 = 1f;
            GameManager.Instance.probabilityCompanion3 = 1f;
        }
        else if (battleManager.companion2 == user)
        {
            GameManager.Instance.probabilityCompanion1 = 1f;
            GameManager.Instance.probabilityCompanion2 = 10f;
            GameManager.Instance.probabilityCompanion3 = 1f;
        }
        else
        {
            // Solo or unknown party slot. Defense buff still matters.
            Debug.Log("[Taunt] Aggro weights not changed because user was not companion1 or companion2.");
        }
    }

    public static void ClearTauntBuff(Character user)
    {
        if (user == null)
            return;

        if (!activeDefenseBuffs.ContainsKey(user))
            return;

        user.defensePower -= activeDefenseBuffs[user];
        activeDefenseBuffs.Remove(user);
    }
}