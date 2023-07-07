using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    public SlimeAttackSkill()
    {
        name = "Slime Attack";
        description = "The slime attacks the player. The damage can be reduced by timely action.";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {

        Debug.Log("using basic attack");
        // Start the attack animation
        user.animator.SetTrigger("SlimeAttackTrigger");

        // Create a timing window for the player to reduce damage
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(0.2f, 0.5f, 0.2f, 0.3f, (result) =>
        {
            if (result)
            {
                // The player succeeded the timing event - reduce the damage of the attack
                Debug.Log("Damage reduced!");
                target.TakeDamage(user.damage - 1 ); // Reduce damage by 1
            }
            else
            {
                // The player missed the timing event - take full damage
                target.TakeDamage(user.damage);
            }

            user.currentSkill.skillExecutionComplete = true;  // Signal that skill execution is complete
        }));
    }
}
