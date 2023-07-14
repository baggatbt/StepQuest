using System.Collections;
using UnityEngine;

//A test skill to allow the enemy to work within the battle system

public class SimpleAttackSkill : Skill
{
    public SimpleAttackSkill()
    {
        name = "Simple Attack";
        description = "Enemy hits the player with a simple attack";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        Debug.Log("Making it into simple execute");
        // Perform the attack on the target
        target.TakeDamage(user.damage);

        Debug.Log("Enemy attacked with Simple Attack");

        // Set the flag to indicate skill execution is complete
        user.currentSkill.skillExecutionComplete = true; 

        yield return null;
    }
}