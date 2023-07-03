using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Character player;
    public Character enemy;
    private BattleState state;  // State manager for player turns and enemy turns
     public int enemyAttackCount = 0; //Counts how many times enemy has attacked, using for special skill activation testing

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
        yield return new WaitUntil(() => player.isAttacking == false);

        if (player.currentSkill != null)
        {
            yield return player.currentSkill.Execute(player, enemy, this);
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
            enemy.TakeDamage(player.damage);
        }

        yield return StartCoroutine(player.ReturnToPosition());

        if (enemy.health <= 0)
        {
            Debug.Log("You win, let's celebrate");
        }
        else
        {
            state = BattleState.EnemyTurn;
            EnemyAttack();
        }
    }

    public IEnumerator EnemyAttackCoroutine()
    {
        yield return StartCoroutine(enemy.MoveToTarget());
        yield return new WaitUntil(() => enemy.isAttacking == false);

        if (enemy.currentSkill != null)
        {
            Debug.Log("Enemy begins executing skill");
            yield return enemy.currentSkill.Execute(enemy, player, this);
            Debug.Log("Enemy has finished executing skill");
        }
        else
        {
            Debug.Log("test enemy attack performed - THis should not happen");
            player.TakeDamage(enemy.damage);
        }

        yield return StartCoroutine(enemy.ReturnToPosition());

        if (player.health <= 0)
        {
            Debug.Log("You lose ya jabroni");
        }
        else
        {
            state = BattleState.PlayerTurn;
        }
    }

    public void EnemyAttack()
{
    if (!player.isAttacking && !enemy.isAttacking && state == BattleState.EnemyTurn)
    {
        if (enemyAttackCount == 2)
        {
            // Use special skill if the attack count is a multiple of attacksBeforeSpecial
            enemy.currentSkill = enemy.specialSkill;
        }
        else
        {
            // Otherwise, use normal skill
            enemy.currentSkill = enemy.normalSkill;
        }

        StartCoroutine(EnemyAttackCoroutine());

        enemyAttackCount++; // Increase the attack count after each attack
    }
}




    public IEnumerator PlayerActiveTimeEvent(float windowStart, float windowEnd, System.Action<bool> callback)
    {
        float totalWindowDuration = windowEnd;
        float timer = totalWindowDuration;
        bool successCallbackCalled = false;

        while (timer > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float elapsedTime = totalWindowDuration - timer;
                Debug.Log(elapsedTime);

                if (elapsedTime >= windowStart && elapsedTime <= windowEnd)
                {
                    Debug.Log("Within window");
                    Debug.Log(elapsedTime);
                    callback?.Invoke(true);
                    successCallbackCalled = true;
                }
                else
                {
                    callback?.Invoke(false);
                    yield break;
                }
            }

            timer -= Time.deltaTime;

            if (timer >= windowStart && timer <= windowEnd)
            {
                Debug.Log("Starting Flash");
                StartCoroutine(FlashWhite(player));
            }

            yield return null;
        }

        // If timer has expired and success callback wasn't called, trigger failure callback
        if (timer <= 0 && !successCallbackCalled)
        {
            callback?.Invoke(false);
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
