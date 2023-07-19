using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BattleManager : MonoBehaviour
{
    public Character player;
    public Character enemy;
    private BattleState state;
    public int enemyAttackCount = 0;
    public GameObject currentTarget;
    public HoldReleaseSlider holdReleaseSlider;

    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn
    }

    public GameObject outerCircle;
    public GameObject innerCircle;
    public GameObject ExpGainedText;
    public GameObject GoldGainedText;

    public GameObject endOfBattlePanel;

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
        // Move to the target before executing the attack
        
        if (player.currentSkill.requiresMovement)
        {
        yield return player.MoveToTarget();
        }
        
        // Execute the attack
        yield return player.currentSkill.Execute(player, enemy, this);

        yield return new WaitUntil(() => !player.isAttacking);

            // Return to the original position after the attack
            if (player.currentSkill.requiresMovement)
            {
                yield return player.ReturnToPosition();
            }
    }
    else
    {
        Debug.Log("Standard Attack Performed - This should not happen");
    }

        if (enemy.health <= 0)
        {
            Debug.Log("You win, let's celebrate");
            endOfBattlePanel.SetActive(true);
            Debug.Log("enemy before EndOfBattleRewards: " + enemy);
            EndOfBattleRewards((Enemy)enemy);
        }

        else
        {
        state = BattleState.EnemyTurn;
        yield return new WaitForSeconds(1.0f); // The delay until the enemy attacks
        EnemyAttack();
    }
}



     public IEnumerator EnemyAttackCoroutine()
    {
        yield return new WaitUntil(() => player.isAttacking == false);

        if (enemy.currentSkill != null)
        {
            if (enemy.currentSkill.requiresMovement)
            {
                yield return enemy.MoveToTarget();
            }
            yield return enemy.currentSkill.Execute(enemy, player, this);

            if (enemy.currentSkill.requiresMovement)
            {
                yield return enemy.ReturnToPosition();
            }
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
            player.TakeDamage(enemy.damage);
            
        }

        if (player.health <= 0)
        {
            Debug.Log("You lose ya jabroni");
            SceneManager.LoadScene("CharacterInfoPage");
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

    public IEnumerator PlayerHoldReleaseTimeEvent(float holdStart, float holdEnd, Action<TimingEventResult> callback)
    {
        float totalHoldDuration = holdEnd - holdStart;
        float holdTimer = 0;

        holdReleaseSlider.ResetSlider(); // Reset the slider at the start of the hold event

        while (holdTimer < totalHoldDuration)
        {
            if (Input.GetMouseButton(0)) // Button is currently held down
            {
                holdTimer += Time.deltaTime;

                // Update the slider value as the hold time increases
                holdReleaseSlider.UpdateSlider(holdTimer / totalHoldDuration);
            }

            if (Input.GetMouseButtonUp(0)) // Button was just released
            {
                break;
            }

            yield return null;
        }

        TimingEventResult result;

        if (holdTimer < totalHoldDuration / 3)
        {
            Debug.Log("Button was released too early. Miss!");
            result = TimingEventResult.Miss;
        }
        else if (holdTimer < 2 * totalHoldDuration / 3)
        {
            Debug.Log("Button was released early. Good!");
            result = TimingEventResult.Good;
        }
        else
        {
            Debug.Log("Button was held for the full duration. Perfect!");
            result = TimingEventResult.Perfect;
        }

        callback(result);
        holdReleaseSlider.ResetSlider(); // Reset the slider at the end of the hold event
    }


    public BattleState GetState()
    {
        return state;
    }

    public void EndOfBattleRewards(Enemy enemy)
    {
       
        TextMeshProUGUI expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
        Debug.Log(enemy.expReward);
        expGainedTextComponent.text = enemy.expReward.ToString();

        TextMeshProUGUI goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
        goldGainedTextComponent.text = enemy.goldReward.ToString();
    }

   
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // Perform the raycast and get the hit information
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            
            // Check if the raycast hits an enemy object
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                // Set the enemy object as the currentTarget
                currentTarget = hit.collider.gameObject;

                // Do something with the currentTarget
                Debug.Log("Current target: " + currentTarget.name);
            }
        }
    }



}