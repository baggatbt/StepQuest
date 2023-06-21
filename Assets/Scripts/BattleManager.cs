using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Character player;
    public Character enemy;
    private BattleState state;  // State manager for player turns and enemy turns

    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn
    }

    private void Start()
    {
        state = BattleState.PlayerTurn;  // Start with the player's turn
    }

    public void PlayerAttack()
    {
        if (state == BattleState.PlayerTurn && !player.isAttacking && !enemy.isAttacking)
        {
            StartCoroutine(PlayerAttackCoroutine(null));
        }
    }

    public IEnumerator PlayerAttackCoroutine(System.Action successCallback)
{
    yield return StartCoroutine(player.MoveToTarget());

    

    // Wait until player has reached the target
    yield return new WaitUntil(() => player.isAttacking == false);
    

    // Check if the player is using a skill
    if (player.currentSkill != null)
    {
        yield return player.currentSkill.Execute(player, enemy, this);
    }
    else
    {
        // Perform standard attack
        Debug.Log("Standard Attack Performed - This should not happen");
        enemy.TakeDamage(player.damage);
    }

    yield return StartCoroutine(player.ReturnToPosition());  // Player returns to its position

    if (enemy.health <= 0)
    {
        // Player wins the fight
        Debug.Log("You win, let's celebrate");
    }
    else
    {
        state = BattleState.EnemyTurn;  // Set the state back to the enemy's turn as the enemy is still alive
        EnemyAttack();
    }
}



    IEnumerator EnemyAttackCoroutine()
    {
        // Enemy begins movement to target
        yield return StartCoroutine(enemy.MoveToTarget());

     
        yield return new WaitUntil(() => enemy.isAttacking == false);  // Wait until enemy has reached the target

       
        // If the enemy has a skill, execute it
        if (enemy.currentSkill != null)
        {
            Debug.Log("Enemy begins executing skill");
            yield return enemy.currentSkill.Execute(enemy, player, this);  // Execute enemy's skill
            Debug.Log("Enemy has finished executing skill");
        }
        else
        {
            // If the enemy has no skill, deal damage directly
            player.TakeDamage(enemy.damage);
        }

        
        yield return StartCoroutine(enemy.ReturnToPosition());  // Enemy returns to its position
       
        state = BattleState.PlayerTurn;  // Set the state back to player's turn after enemy's attack

        // Check if the player is still alive after the enemy's attack
        if (player.health <= 0)
        {
            // Game over, player is defeated
            Debug.Log("Game Over. Player is defeated.");
        }
        else
        {
            // If player is still alive, start player's turn
            state = BattleState.PlayerTurn;
        }
    }



    public void EnemyAttack()
    {
        if (!player.isAttacking && !enemy.isAttacking && state == BattleState.EnemyTurn)
        {
            
            StartCoroutine(EnemyAttackCoroutine());
        }
    }

/****DEPRECIATED
    public void UseSkill(Skill skill, Character user, Character target)
    {
        if (state == BattleState.PlayerTurn && !user.isAttacking && !target.isAttacking)
        {
            user.currentSkill = skill;  // Set the current skill being used by the player
            StartCoroutine(skill.Execute(user, target, this));
            // Set the state to EnemyTurn after skill execution
            state = BattleState.EnemyTurn;
        }
    }
    */

/** DEPRECIATED
    public float activeTimeWindowDuration = 0.0f; // Default active time window duration

    public void SetActiveTimeWindow(float windowDuration)
    {
        activeTimeWindowDuration = windowDuration;
    }

    */

    public IEnumerator PlayerActiveTimeEvent(float windowStart, float windowEnd, System.Action<bool> callback)
{
    float totalWindowDuration = windowEnd;  // Total duration is determined by the end time of the last window
    float timer = totalWindowDuration;

    while (timer > 0)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check the timing window for this skill
            float elapsedTime = totalWindowDuration - timer;
            Debug.Log(elapsedTime);

            if (elapsedTime >= windowStart && elapsedTime <= windowEnd)
            {
                Debug.Log("Within window");
                Debug.Log(elapsedTime);
                callback?.Invoke(true);  // Success within the timing window
            }
            else
            {
                callback?.Invoke(false);  // Failed outside the timing window
                yield break;  // Exit the coroutine
            }
        }

        timer -= Time.deltaTime;

        // Flash white to indicate tapping at this moment will be a success
        if (timer >= windowStart && timer <= windowEnd)
        {
            Debug.Log("Starting Flash");
            StartCoroutine(FlashWhite(player));
        }

        yield return null;
    }
}


    public IEnumerator FlashWhite(Character character)
    {
        character.spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        character.spriteRenderer.color = Color.cyan;
    }

    public BattleState GetState()
    {
        return state;
    }
}
