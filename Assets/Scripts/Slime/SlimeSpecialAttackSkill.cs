using System.Collections;
using UnityEngine;

public class SlimeSpecialAttackSkill : Skill
{
    // Constants for the timing window for player block
    private const float LEFT_CLICK_START = 0.5f;
    private const float LEFT_CLICK_END = 1.5f;
    private const float RIGHT_CLICK_START = 0.8f;
    private const float RIGHT_CLICK_END = 1.2f;

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
        // Insert your damage calculation logic here. For now, let's return a simple value.
        return user.damage;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {

        Debug.Log("using special attack");

        // Reset the skillExecutionComplete flag at the start
        skillExecutionComplete = false;

        user.animator.SetTrigger("SlimeSpecialAttackTrigger");

        bool timingSuccess = false;

        // Player has a chance to block the attack
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(
        LEFT_CLICK_START, LEFT_CLICK_END, RIGHT_CLICK_START, RIGHT_CLICK_END, (result) =>
        {
            timingSuccess = result;
        }));

        // If the player blocked successfully, do less damage
<<<<<<< Updated upstream
        if (timingSuccess)
=======
        if (timingResult == TimingEventResult.LeftClickSuccess)
>>>>>>> Stashed changes
        {
            Debug.Log("Left click succeeded - Attack blocked!");
            target.TakeDamage(CalculateBlockedDamage(user));
        }
        else if (timingResult == TimingEventResult.RightClickSuccess)
        {
            Debug.Log("Right click succeeded - Attack dodged!");
            // Handle the dodge action here
        }
        else
        {
            // Attack was not blocked or dodged
            target.TakeDamage(CalculateUnblockedDamage(user));
        }

        Debug.Log("Enemy attacked with Slime Special Attack");

        // Set the skillExecutionComplete flag to true at the end
        skillExecutionComplete = true;
    }
}