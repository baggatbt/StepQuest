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
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(0.2f, 0.5f, (result) =>
        {
            if (result == TimingEventResult.Success)
            {
                Debug.Log("Damage reduced!");
                target.TakeDamage(user.damage - 1);
            }
            else if (result == TimingEventResult.RightClickSuccess)
            {
                Debug.Log("Damage dodged!");
                target.TakeDamage(user.damage - user.damage);
            }
            else
            {
                target.TakeDamage(user.damage);
            }

            user.currentSkill.skillExecutionComplete = true;
        }));
    }

}
