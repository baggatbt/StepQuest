using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public Character player;
    public List<Character> enemies = new List<Character>();
    private BattleState state;
    public int enemyAttackCount = 0;
    public GameObject currentTarget;
    public HoldReleaseSlider holdReleaseSlider;
    public Slider[] healthBars; //set these in the inspector


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

    public EnemySpawnController enemySpawnController;

    private Vector3 outerCircleInitialScale;
    private Vector3 innerCircleInitialScale;

    public Transform[] enemySpawnPoints; // enemySpawnPoint1, enemySpawnPoint2....

    [SerializeField]
    private BattleConfig currentBattleConfig;


   private void Start()
{
    state = BattleState.PlayerTurn;
    outerCircleInitialScale = outerCircle.transform.localScale;
    innerCircleInitialScale = innerCircle.transform.localScale;

    outerCircle.SetActive(false);
    innerCircle.SetActive(false);

    if (currentBattleConfig != null)
    {
        StartBattle(currentBattleConfig);
    }
    else
    {
        Debug.LogError("No battle configuration set!");
        // Handle error or default configuration
    }
}



    public void StartBattle(BattleConfig config)
{
    // Use config.enemyType and config.maxEnemiesToSpawn to set up your battle
    // You can use the enemySpawnController to spawn the desired enemy type and number
    
    for (int i = 0; i < config.maxEnemiesToSpawn; i++)
    {
        // Spawn enemies based on the config.enemyType
       enemySpawnController.SpawnEnemiesFromPool(config.poolName, 1, enemySpawnPoints[i], healthBars[i]);

    }
    // Continue with any other setup like setting backgrounds, play music, etc.
}



    public void PlayerAttack()
    {
        if (state == BattleState.PlayerTurn && !player.isAttacking && currentTarget && !currentTarget.GetComponent<Character>().isAttacking)
        {
            StartCoroutine(PlayerAttackCoroutine(null));
        }
    }

    public IEnumerator PlayerAttackCoroutine(System.Action successCallback)
    {
        Character targetEnemy = currentTarget.GetComponent<Character>();
        yield return new WaitUntil(() => targetEnemy.isAttacking == false);

        if (player.currentSkill != null)
        {
            if (player.currentSkill.requiresMovement)
            {
                yield return player.MoveToTarget();
            }
            
            yield return player.currentSkill.Execute(player, targetEnemy, this);

            yield return new WaitUntil(() => !player.isAttacking);

            if (player.currentSkill.requiresMovement)
            {
                yield return player.ReturnToPosition();
            }
            Debug.Log("Got this far, 1"); // Add this
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
        }

        if (targetEnemy.health <= 0)
        {
            Debug.Log(targetEnemy.name + " is defeated!");
            enemies.Remove(targetEnemy);

            if (enemies.Count == 0)
            {
                Debug.Log("You win, let's celebrate");
                endOfBattlePanel.SetActive(true);
                EndOfBattleRewards((Enemy)targetEnemy);
            }
            
        }
        else
        {
            Debug.Log("Got this far, 2"); // Add this
            state = BattleState.EnemyTurn;
            Debug.Log("Changed state to EnemyTurn"); // Add this
            yield return new WaitForSeconds(1.0f);
            EnemyAttack();
        }
    }

    public void EnemyAttack()
    {
        if (enemies.Count == 0)
        {
            Debug.LogError("There are no enemies left to attack!");
            return;
        }

        Character attackingEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];
        Debug.Log(attackingEnemy);

        if (!player.isAttacking && !attackingEnemy.isAttacking && state == BattleState.EnemyTurn)
        {
            attackingEnemy.currentSkill = attackingEnemy.normalSkill;
            Debug.Log("The enemy is starting to attack");
            StartCoroutine(EnemyAttackCoroutine(attackingEnemy));
        }
    }


    public IEnumerator EnemyAttackCoroutine(Character attackingEnemy)
    {
        yield return new WaitUntil(() => player.isAttacking == false);

        if (attackingEnemy.currentSkill != null)
        {
            if (attackingEnemy.currentSkill.requiresMovement)
            {
                yield return attackingEnemy.MoveToTarget();
            }
            
            yield return attackingEnemy.currentSkill.Execute(attackingEnemy, player, this);

            if (attackingEnemy.currentSkill.requiresMovement)
            {
                yield return attackingEnemy.ReturnToPosition();
            }
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
            player.TakeDamage(attackingEnemy.damage);
        }

        if (player.health <= 0)
        {
            Debug.Log("You lose ya jabroni");
            SceneManager.LoadScene("CharacterInfoPage");
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            state = BattleState.PlayerTurn;
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

    public void EndOfBattleRewards(Enemy defeatedEnemy)
    {
        TextMeshProUGUI expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
        Debug.Log(defeatedEnemy.expReward);
        expGainedTextComponent.text = defeatedEnemy.expReward.ToString();

        TextMeshProUGUI goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
        goldGainedTextComponent.text = defeatedEnemy.goldReward.ToString();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                currentTarget = hit.collider.gameObject;
                Debug.Log("Current target: " + currentTarget.name);
            }
        }
    }

    public bool IsAnyEnemyAttacking()
{
    foreach (var enemy in enemies)
    {
        if (enemy.isAttacking)
            return true;
    }
    return false;
}


}