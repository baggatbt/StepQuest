using System.Collections;
using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Character player;
    public Character enemy;
    private BattleState state;
    public int enemyAttackCount = 0;
    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn
    }

    public GameObject outerCircle;
    public GameObject innerCircle;

    private Vector3 outerCircleInitialScale;
    private Vector3 innerCircleInitialScale;

    private void Start()
    {
        state = BattleState.PlayerTurn;
        outerCircleInitialScale = outerCircle.transform.localScale;
        innerCircleInitialScale = innerCircle.transform.localScale;
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
       // yield return StartCoroutine(player.MoveToTarget());
        yield return new WaitUntil(() => player.isAttacking == false);

        if (player.currentSkill != null)
        {
           
            yield return player.currentSkill.Execute(player, enemy, this);
           
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
            
        }

       // yield return StartCoroutine(player.ReturnToPosition());

        if (enemy.health <= 0)
        {
            Debug.Log("You win, let's celebrate");
        }
        else
        {
            state = BattleState.EnemyTurn;
            yield return new WaitForSeconds(1.5f); //The delay util the enemy attacks
            EnemyAttack();
        }
    }

    public IEnumerator EnemyAttackCoroutine()
    {
        //yield return StartCoroutine(enemy.MoveToTarget());
        yield return new WaitUntil(() => enemy.isAttacking == false);

        if (enemy.currentSkill != null)
        {
            Debug.Log("Enemy begins executing skill");
            yield return enemy.currentSkill.Execute(enemy, player, this);
            Debug.Log("Enemy has finished executing skill");
        }
        else
        {
            Debug.Log("test enemy attack performed - This should not happen");
            player.TakeDamage(enemy.damage);
        }

       // yield return StartCoroutine(enemy.ReturnToPosition());

        if (player.health <= 0)
        {
            Debug.Log("You lose ya jabroni");
        }
        else
        {
            yield return new WaitForSeconds(1.5f); //The delay util the player attacks
            state = BattleState.PlayerTurn;
        }
    }

    public void EnemyAttack()
    {
        if (!player.isAttacking && !enemy.isAttacking && state == BattleState.EnemyTurn)
        {
            enemy.currentSkill = enemy.normalSkill;
            StartCoroutine(EnemyAttackCoroutine());
            enemyAttackCount++;
        }
    }

    public IEnumerator PlayerActiveTimeEvent(float windowStart, float windowEnd, System.Action<TimingEventResult> callback)
    {
        float totalWindowDuration = windowEnd - windowStart;
        float timer = 0;
        bool buttonClicked = false;

        // Start the inner circle at a very small size
        Vector3 innerCircleInitialScale = new Vector3(0.01f, 0.01f, 0.01f);

        try
        {
            while (timer < totalWindowDuration)
            {
                float progress = timer / totalWindowDuration;
                innerCircle.transform.localScale = Vector3.Lerp(innerCircleInitialScale, outerCircleInitialScale, progress);

                if (Input.GetMouseButtonDown(0))
                {
                    buttonClicked = true;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            TimingEventResult result;
            if (buttonClicked)
            {
                result = GetTimingAccuracy(innerCircle.transform.localScale, outerCircle.transform.localScale);
            }
            else
            {
                Debug.Log("Timing Missed!");
                result = TimingEventResult.Miss;
            }

            callback(result);
        }
        finally
        {
            // Reset the scale of the inner circle, ensuring it always happens even if the coroutine is interrupted
            innerCircle.transform.localScale = innerCircleInitialScale;
        }
    }






    private TimingEventResult GetTimingAccuracy(Vector3 innerCircleScale, Vector3 outerCircleScale)
    {
        float scaleRatio = innerCircleScale.x / outerCircleScale.x;
        float perfectThreshold = 0.9f;
        float greatThreshold = 0.75f;
        float goodThreshold = 0.5f;

        if (scaleRatio >= perfectThreshold)
        {
            return TimingEventResult.Perfect;
        }
        else if (scaleRatio >= greatThreshold)
        {
            return TimingEventResult.Great;
        }
        else if (scaleRatio >= goodThreshold)
        {
            return TimingEventResult.Good;
        }
        else
        {
            return TimingEventResult.Miss;
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