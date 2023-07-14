using System.Collections;
using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Character player;
    public Character enemy;
    private BattleState state;
    public int enemyAttackCount = 0;

    public HoldReleaseSlider holdReleaseSlider;

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

         // Dont show the timing circle yet
        outerCircle.SetActive(false);
        innerCircle.SetActive(false);
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
        yield return new WaitUntil(() => enemy.isAttacking == false);

        if (player.currentSkill != null)
        {
            yield return player.currentSkill.Execute(player, enemy, this);
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
        }

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
        yield return new WaitUntil(() => player.isAttacking == false);

        if (enemy.currentSkill != null)
        {
            yield return enemy.currentSkill.Execute(enemy, player, this);
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
            player.TakeDamage(enemy.damage);
        }

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
      // Enable the timing circles when the event starts
        outerCircle.SetActive(true);
        innerCircle.SetActive(true);

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
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        TimingEventResult result;
        if (buttonClicked)
        {
            if (timer >= windowStart)
            {
                result = GetTimingAccuracy(innerCircle.transform.localScale, outerCircle.transform.localScale);
            }
            else
            {
                Debug.Log("Timing Missed!");
                result = TimingEventResult.Miss;
            }
        }
        else
        {
            Debug.Log("No input detected. Missed!");
            result = TimingEventResult.Miss;
        }

        callback(result);
    }
    finally
    {
        // Reset the scale of the inner circle, ensuring it always happens even if the coroutine is interrupted
        innerCircle.transform.localScale = innerCircleInitialScale;
    }
    // Disable the timing circles when the event ends
        outerCircle.SetActive(false);
        innerCircle.SetActive(false);
}


    
    private TimingEventResult GetTimingAccuracy(Vector3 innerCircleScale, Vector3 outerCircleScale)
    {
        float scaleRatio = innerCircleScale.x / outerCircleScale.x;
        float perfectThreshold = 0.9f;
        float goodThreshold = 0.5f;

        if (scaleRatio >= perfectThreshold)
        {
            return TimingEventResult.Perfect;
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

    public IEnumerator PlayerHoldReleaseTimeEvent(float holdStart, float releaseStart, float releaseEnd, Action<TimingEventResult> callback)
{
    float totalHoldDuration = releaseEnd - holdStart;
    float totalReleaseDuration = releaseEnd - releaseStart;
    float holdTimer = 0;
    float releaseTimer = 0;
    bool buttonHeld = false;
    bool buttonReleased = false;

    holdReleaseSlider.ResetSlider(); // Reset the slider at the start of the hold and release event

    while (holdTimer < totalHoldDuration)
    {
        if (Input.GetMouseButtonDown(0))
        {
            buttonHeld = true;
            break;
        }

        holdTimer += Time.deltaTime;

        // Update the slider value as the hold time increases
        holdReleaseSlider.UpdateSlider(holdTimer / totalHoldDuration);

        yield return null;
    }

    TimingEventResult result;

    if (!buttonHeld)
    {
        Debug.Log("Button was not held. Missed!");
        result = TimingEventResult.Miss;
    }
    else
    {
        while (releaseTimer < totalReleaseDuration)
        {
            if (Input.GetMouseButtonUp(0))
            {
                buttonReleased = true;
                break;
            }

            releaseTimer += Time.deltaTime;

            // Update the slider value as the release time increases
            holdReleaseSlider.UpdateSlider(releaseTimer / totalReleaseDuration);

            yield return null;
        }

        if (!buttonReleased)
        {
            Debug.Log("Button was not released. Good!");
            result = TimingEventResult.Good;
        }
        else
        {
            if (releaseTimer < totalReleaseDuration / 2)
            {
                Debug.Log("Button was released early. Miss!");
                result = TimingEventResult.Miss;
            }
            else
            {
                Debug.Log("Button was released at perfect time. Perfect!");
                result = TimingEventResult.Perfect;
            }
        }
    }

    callback(result);
    holdReleaseSlider.ResetSlider(); // Reset the slider at the end of the hold and release event
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