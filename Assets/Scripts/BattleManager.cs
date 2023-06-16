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
            StartCoroutine(PlayerAttackCoroutine());
        }
    }

    IEnumerator PlayerAttackCoroutine()
    {
        player.isAttacking = true;
        StartCoroutine(player.MoveToTarget());
        yield return new WaitUntil(() => player.isAttacking == false);

        // Check if the player is using a skill
        if (player.currentSkill != null)
        {
            yield return player.currentSkill.Execute(player, enemy, this);
        }
        else
        {
            // Perform standard attack
            enemy.TakeDamage(player.damage);
        }

        if (enemy.health > 0)
        {
            state = BattleState.EnemyTurn;  // Set the state to enemy's turn
            EnemyAttack();
        }
        else
        {
            state = BattleState.PlayerTurn;  // Set the state back to player's turn as enemy is defeated
        }
    }

    IEnumerator EnemyAttackCoroutine()
{
    enemy.isAttacking = true;
    StartCoroutine(enemy.MoveToTarget());
    yield return new WaitUntil(() => enemy.isAttacking == false);

    player.TakeDamage(enemy.damage);

    state = BattleState.PlayerTurn;  // Set the state back to player's turn after enemy's attack
}


    public void EnemyAttack()
    {
        if (state == BattleState.EnemyTurn)
        {
            StartCoroutine(EnemyAttackCoroutine());
        }
    }

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

   public float activeTimeWindowDuration = 0.0f; // Default active time window duration

    public void SetActiveTimeWindow(float windowDuration)
    {
        activeTimeWindowDuration = windowDuration;
    }

    public IEnumerator PlayerActiveTimeEvent(float[] windowStarts, float[] windowEnds, System.Action<bool> callback)
{
    float totalWindowDuration = windowEnds[windowEnds.Length - 1];  // Total duration is determined by the end time of the last window
    float timer = totalWindowDuration;
    int currentWindowIndex = 0;

    while (timer > 0)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check the timing window for this skill
            float elapsedTime = totalWindowDuration - timer;
            Debug.Log(elapsedTime);

            if (elapsedTime >= windowStarts[currentWindowIndex] && elapsedTime <= windowEnds[currentWindowIndex])
            {
                callback?.Invoke(true);  // Success within the timing window
                currentWindowIndex++;  // Move to the next timing window
            }
            else
            {
                callback?.Invoke(false);  // Failed outside the timing window
                yield break;  // Exit the coroutine
            }

            if (currentWindowIndex >= windowStarts.Length)
            {
                yield break;  // Exit the coroutine if all timing windows have been successfully hit
            }
        }

        timer -= Time.deltaTime;

        if (timer >= windowStarts[currentWindowIndex] && timer <= windowEnds[currentWindowIndex])
        {
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
