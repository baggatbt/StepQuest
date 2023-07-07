using System.Collections;
using UnityEngine;

public class SlimeSpecialAttackSkill : Skill
{
    // Constants for the timing window for player block
    private const float WINDOW_START = 0.5f;
    private const float WINDOW_END = 1.5f;

    public SlimeSpecialAttackSkill()
    {
        name = "Slime Special Attack";
        description = "The slime hits hard, unless the attack is blocked";
    }

    // Damage calculation methods
    private int CalculateUnblockedDamage(Character user)
    {
        // Insert your damage calculation logic here. For now, let's return a simple value.
        return user.damage + 1;
    }

    private int CalculateBlockedDamage(Character user)
    {
        
        return user.damage;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        Debug.Log("using special attack");

        // Reset the skillExecutionComplete flag at the start
        skillExecutionComplete = false;

        user.animator.SetTrigger("SlimeSpecialAttackTrigger");

        TimingEventResult timingResult = TimingEventResult.Failure;

        // Player has a chance to block the attack
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(WINDOW_START, WINDOW_END, (result) =>
        {
            timingResult = result;
        }));

        // If the player blocked successfully, do less damage
        if (timingResult == TimingEventResult.Success)
        {
            Debug.Log("Attack blocked!");
            target.TakeDamage(CalculateBlockedDamage(user));
        }
        else
        {
            // Otherwise, do full damage
            target.TakeDamage(CalculateUnblockedDamage(user));
        }

        Debug.Log("Enemy attacked with Slime Special Attack");

        // Set the skillExecutionComplete flag to true at the end
        skillExecutionComplete = true;
    }

}