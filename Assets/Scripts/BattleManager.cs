using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public Character player;
    public Character companion;
    public PlayerData playerCharacterData;
    public List<Character> enemies = new List<Character>();
    public GameObject knightPrefab;
    private BattleState state;
    public int enemyAttackCount = 0;
    public GameObject currentTarget;
    public HoldReleaseSlider holdReleaseSlider;
    public Slider[] healthBars; //set these in the inspector
    public Skill requestedSkill; // The skill the player chooses next during an ongoing attack.
    public Queue<Skill> skillQueue = new Queue<Skill>();
    public Button[] skillButtons; // An array of buttons representing skill slots
    public Button launchAttacksButton; 
    public StatusEffectController statusEffectController;
    




    public enum BattleState
    {
        PlayerTurn,
        CompanionTurn,
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
    private TextMeshProUGUI expGainedTextComponent;
    private TextMeshProUGUI goldGainedTextComponent;

    private Camera mainCamera;


    public Transform[] enemySpawnPoints; // enemySpawnPoint1, enemySpawnPoint2....


    //public PlayerSpawnController playerSpawnController;
   // public Transform playerSpawnPoint;

    [SerializeField]
    private BattleConfig currentBattleConfig;

    public BattleState State
    {
        get { return state; }
        private set 
        {
            state = value;
            switch (state)
            {
                case BattleState.PlayerTurn:
                    Debug.Log("Player Turn Started!");
                    break;
                case BattleState.CompanionTurn:
                    Debug.Log("Companion Turn Started!");
                    break;
                case BattleState.EnemyTurn:
                    Debug.Log("Enemy Turn Started!");
                    StartEnemyTurn();
                    break;
            }
        }
    }


   private void Start()
{
    State = BattleState.PlayerTurn;
    outerCircleInitialScale = outerCircle.transform.localScale;
    innerCircleInitialScale = innerCircle.transform.localScale;

    outerCircle.SetActive(false);
    innerCircle.SetActive(false);

    // Cache the components
    expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
    goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
    mainCamera = Camera.main;


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

    private void StartEnemyTurn()
{
    EnemyAttack();
}


    private void ChangeState(BattleState newState)
    {
        State = newState;
        if (State == BattleState.PlayerTurn || State == BattleState.CompanionTurn)
        {
            EnableAllButtons();
        }
    }

    public void DisableAllButtons()
    {
        foreach (Button btn in skillButtons)
        {
            btn.interactable = false;
        }
        launchAttacksButton.interactable = false;
    }
   
    public void EnableAllButtons()
    {
        foreach (Button btn in skillButtons)
        {
            btn.interactable = true;
        }
        launchAttacksButton.interactable = true;
    }

    public void MoveCirclesToTarget(Character target)
{
            // Move the circles to the targets position
            outerCircle.transform.position = currentTarget.transform.position;
            innerCircle.transform.position = currentTarget.transform.position;
       
}


    public void StartBattle(BattleConfig config)
    {
        // Use config.poolName and config.maxEnemiesToSpawn to set up battle
        // use the enemySpawnController to spawn the desired enemy type and number
    
        for (int i = 0; i < config.maxEnemiesToSpawn; i++)
        {
            // Spawn enemies based on the config.poolName
            Character spawnedEnemy = enemySpawnController.SpawnEnemiesFromPool(config.poolName, 1, enemySpawnPoints[i], healthBars[i]);

            // Add spawned enemy to the list
            if (spawnedEnemy != null)
            {
                enemies.Add(spawnedEnemy);
            }
        }
        // TODO: Continue with any other setup like setting backgrounds, play music, etc.
    }

        public void ExecuteQueuedSkills()
    {
        if (currentTarget != null && state == BattleState.PlayerTurn)
        {
        DisableAllButtons();
        StartCoroutine(ExecuteAllSkillsCoroutine());
        }
        else
        {
        Debug.Log("No target selected");
        }
    }

    public void CompanionExecuteQueuedSkills()
    {
        if (currentTarget != null && state == BattleState.CompanionTurn)
        {
        DisableAllButtons();
        StartCoroutine(CompanionExecuteAllSkillsCoroutine());
        }
        else
        {
        Debug.Log("No target selected");
        }
    }

    public int skillsExecuted = 0;
    private IEnumerator ExecuteAllSkillsCoroutine()
    {
        if (currentTarget == null)
        {
            Debug.Log("Pick a target!");
        }
        else
        {
        Debug.Log("Executing queued skills. Current queue size before execution: " + skillQueue.Count);

        // Dequeue skills one by one and execute them
        Debug.Log("Starting skill execution loop");
        int skillsExecuted = 0;
        while(skillQueue.Count > 0)
        {
            Debug.Log("Skill execution of  " + skillQueue.Count);
            Skill skill = skillQueue.Dequeue();
            Debug.Log("Executing skill: " + skill.skillName);
            player.currentSkill = skill;
            skillsExecuted++;
            yield return StartCoroutine(PlayerAction());
            
        }
        
        
        Debug.Log("Cleared the skill queue after execution.");
        skillsExecuted = 0;
        yield return new WaitUntil(() => player.isAttacking == false);
        // Move player back to their original position after all skills executed.
        //yield return StartCoroutine(zoomEffect.ZoomOutEffect());
        yield return player.ReturnToPosition();
        
        if (companion == null)
        {
            ChangeState(BattleState.EnemyTurn);
        }
        else
        {
        ChangeState(BattleState.CompanionTurn);
        }
        }
    }

    public int companionSkillsExecuted = 0;

    private IEnumerator CompanionExecuteAllSkillsCoroutine()
    {
        Debug.Log("Executing queued skills. Current queue size before execution: " + skillQueue.Count);

        // Dequeue skills one by one and execute them
        Debug.Log("Starting skill execution loop");
        int companionSkillsExecuted = 0;
        while(skillQueue.Count > 0)
        {
            Debug.Log("Skill execution of  " + skillQueue.Count);
            Skill skill = skillQueue.Dequeue();
            Debug.Log("Executing skill: " + skill.skillName);
            companion.currentSkill = skill;
            companionSkillsExecuted++;
            yield return StartCoroutine(CompanionAction());
            
        }
        
        
        Debug.Log("Cleared the skill queue after execution.");
        skillsExecuted = 0;
        yield return new WaitUntil(() => companion.isAttacking == false);
        // Move player back to their original position after all skills executed.
       // yield return StartCoroutine(zoomEffect.ZoomOutEffect());
        yield return new WaitForSeconds(1.0f);
        yield return companion.ReturnToPosition();
        

        ChangeState(BattleState.EnemyTurn);
    }

    //public ZoomEffect zoomEffect;


    public IEnumerator PlayerAction()
{
    if (state == BattleState.PlayerTurn && currentTarget)
    {
        Debug.Log("PlayerAction() being called");

        // Start zoom effect
        //StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position)); 

        if (player.currentSkill.requiresMovement && skillsExecuted == 0)
        {
            yield return StartCoroutine(PlayerMoveAndAttackCoroutine());
        }
        else
        {
            yield return StartCoroutine(PlayerAttackCoroutine(null));
        }

    }
}

public IEnumerator CompanionAction()
{
    if (state == BattleState.CompanionTurn && currentTarget)
    {
        Debug.Log("CompanionAction() being called");

        // Start zoom effect
       // StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position)); 

        if (companion.currentSkill.requiresMovement && companionSkillsExecuted == 0)
        {
            yield return StartCoroutine(CompanionMoveAndAttackCoroutine());
        }
        else
        {
            yield return StartCoroutine(CompanionAttackCoroutine(null));
        }

    }
}



    public IEnumerator PlayerMoveAndAttackCoroutine()
{
    // Only move if the player is not already at the target
    if (player.transform.position != currentTarget.transform.position)
    {
        yield return player.MoveToTarget();
        
          
    }

    yield return StartCoroutine(PlayerAttackCoroutine(null));
}

public IEnumerator CompanionMoveAndAttackCoroutine()
{
    // Only move if the player is not already at the target
    if (companion.transform.position != currentTarget.transform.position)
    {
        yield return companion.MoveToTarget();
        
          
    }

    yield return StartCoroutine(CompanionAttackCoroutine(null));
}

    public IEnumerator PlayerAttackCoroutine(System.Action successCallback)
{
    Character targetEnemy = currentTarget.GetComponent<Character>();
    yield return new WaitUntil(() => targetEnemy.isAttacking == false);

    if (player.currentSkill != null)
    {
        MoveCirclesToTarget(targetEnemy);
        yield return player.currentSkill.Execute(player, targetEnemy, this);
        
        yield return new WaitForSeconds(0.1f);

        CheckBattleEnd();
    }
    else
    {
        Debug.Log("currentSkill  - This should not happen");
    }
}

public IEnumerator CompanionAttackCoroutine(System.Action successCallback)
{
    Character targetEnemy = currentTarget.GetComponent<Character>();
    yield return new WaitUntil(() => targetEnemy.isAttacking == false);

    if (companion.currentSkill != null)
    {
        MoveCirclesToTarget(targetEnemy);
        yield return companion.currentSkill.Execute(companion, targetEnemy, this);
        
        yield return new WaitForSeconds(0.1f);

        CheckBattleEnd();
    }
    else
    {
        Debug.Log("currentSkill  - This should not happen");
    }
}



    private Queue<Character> enemyTurnQueue = new Queue<Character>();


    public void EnemyAttack()
    {
        // If the queue is empty (or at the start of the enemy turn phase), populate it.
        if (enemyTurnQueue.Count == 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.health > 0) // Assuming you have some isDead flag on enemies
                    enemyTurnQueue.Enqueue(enemy);
            }
        }

        // If all enemies had their turns, it's the player's turn next.
        if (enemyTurnQueue.Count == 0)
        {
            ChangeState(BattleState.PlayerTurn);
            return;
        }

        Character attackingEnemy = enemyTurnQueue.Dequeue();

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
            yield return new WaitForSeconds(1.0f);

           // Check if there are more enemies to take their turns
        if (enemyTurnQueue.Count > 0)
        {
            EnemyAttack();  // Next enemy's turn
        }
        else
        {
            player.GainEnergy(5); //TODO: Instead of hard value, use player stat energyRegenValue
            ChangeState(BattleState.PlayerTurn);  // If all enemies had their turns, it's the player's turn next.
         }
        CheckBattleEnd();
        }
    }
    
   
   public CameraShake cameraShake;


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

    float speedFactor = 1.5f;  // Change this value to adjust speed. Higher means faster.
    

    try
    {
        while (timer < totalWindowDuration)
        {
            float progress = timer / totalWindowDuration;
            innerCircle.transform.localScale = Vector3.Lerp(innerCircleInitialScale, outerCircleInitialScale, progress);

            if (Input.GetMouseButtonDown(0))
            {
                buttonClicked = true;
                StartCoroutine(cameraShake.Shake());

                break;
            }

            timer += Time.deltaTime * speedFactor;
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
                result = TimingEventResult.Miss; //Breaks the queue'd chain if anything misses.
                skillQueue.Clear();
            }
        }
        else
        {
            Debug.Log("No input detected. Missed!");
            result = TimingEventResult.Miss;
            skillQueue.Clear();
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
        holdReleaseSlider.gameObject.SetActive(true);

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
                holdReleaseSlider.gameObject.SetActive(false);  
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

    public void EndOfBattleRewards(List<Character> enemies)
{
    int totalExp = 0;
    int totalGold = 0;

    foreach (Enemy enemy in enemies)
    {
        totalExp += enemy.expReward;
        totalGold += enemy.goldReward;
    }

    PlayerData.Instance.exp += totalExp;
    PlayerData.Instance.gold += totalGold;

    TextMeshProUGUI expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
    expGainedTextComponent.text = totalExp.ToString();

    TextMeshProUGUI goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
    goldGainedTextComponent.text = totalGold.ToString();

    Debug.Log($"Total EXP gained: {totalExp}");
    Debug.Log($"Total Gold gained: {totalGold}");
}



    private void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            currentTarget = hit.collider.gameObject;
            player.attackTarget = currentTarget.transform; //For movement purposes sets the target to the players target
          //  companion.attackTarget = currentTarget.transform;
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

public void CheckBattleEnd()
{
    bool allEnemiesDefeated = true;

    foreach (var enemy in enemies)
    {
        if (enemy.health > 0)  
        {
            allEnemiesDefeated = false;
            break;
        }
    }

    if (allEnemiesDefeated)
    {
        EndOfBattleRewards(enemies);
        endOfBattlePanel.SetActive(true);
        
    }
}



}
