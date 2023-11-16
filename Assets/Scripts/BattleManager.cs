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
    public Character activePlayer;
    public PlayerData playerCharacterData;
    public Player playerClassReference;
    public List<Character> enemies = new List<Character>();
    public GameObject activePlayerIndicator;
    
    
    public GameObject knightPrefab;
    private BattleState state;
    public int enemyAttackCount = 0;
    public GameObject currentTarget;
    public HoldReleaseSlider holdReleaseSlider;
    public Slider[] healthBars; //set these in the inspector for enemies
    public Slider[] energyBars;
    public Skill requestedSkill; // The skill the player chooses next during an ongoing attack.
    public Queue<Skill> skillQueue = new Queue<Skill>();
    public Button[] skillButtons; // An array of buttons for representing skill slots
    public Button launchAttacksButton; 
    public StatusEffectController statusEffectController;
    public TextMeshProUGUI timingFeedbackText;
    




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
                    activePlayer = player;
                    Debug.Log("Player Turn Started!");
                    Debug.Log("Active player from Player1" + activePlayer);
                    break;
                case BattleState.EnemyTurn:
                    Debug.Log("Enemy Turn Started!");
                    StartEnemyTurn();
                    break;
            }
        }
    }

       [SerializeField] private BattleConfig[] battleConfigs;  // List of battle configurations for each stage.
        private int currentStageIndex = 0;  // Index to track the current stage.

    private void Awake()
    {
        activePlayer = player;
    }
   private void Start()
{
    
    State = BattleState.PlayerTurn;
    activePlayer = player;
    Debug.Log("Active player from Player1 in start method"  + activePlayer);
    outerCircleInitialScale = outerCircle.transform.localScale;
    innerCircleInitialScale = innerCircle.transform.localScale;

    outerCircle.SetActive(false);
    activePlayerIndicator.SetActive(false);
    innerCircle.SetActive(false);
    

    // Cache the components
    expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
    goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
    mainCamera = Camera.main;
    // Retrieve the BattleConfig from the GameManager after the scene is loaded
        BattleConfig config = GameManager.Instance.CurrentBattleConfig;
        if (config != null)
        {
            StartBattle(config);
        }
        else
        {
            Debug.LogError("No battle configuration found.");
        }
    



    Debug.Log(activePlayer);
}
    public Button nextBattleButton;
    
    /*
    public void LoadNewConfig(BattleConfig newConfig)
    {
        if (newConfig != null)
        {
            currentBattleConfig = newConfig;
            // Need to clear old battle data or reset states here
            StartBattle(currentBattleConfig);
        }
        else
        {
            Debug.LogError("The new configuration is null!");
        }
    }
    */
   
    

     public void StartBattle(BattleConfig config)
    {
        // Use config.poolName and config.maxEnemiesToSpawn to set up battle
        // use the enemySpawnController to spawn the desired enemy type and number
        endOfBattlePanel.SetActive(false);
        for (int i = 0; i < config.maxEnemiesToSpawn; i++)
        {
            // Spawn enemies based on the config.poolName
            Character spawnedEnemy = enemySpawnController.SpawnEnemiesFromPool(config.poolName, 1, enemySpawnPoints[i], healthBars[i], energyBars[i]);

            // Add spawned enemy to the list
            if (spawnedEnemy != null)
            {
                enemies.Add(spawnedEnemy);
            }
        }
       // if (player.speed >= companion.speed)
            State = BattleState.PlayerTurn;
        
        
        
        // TODO: Continue with any other setup like setting backgrounds, play music, etc.
    }

    public void NextBattle()
    {
        currentStageIndex++;  // Increment the stage index.
        if (currentStageIndex < battleConfigs.Length)
        {
            StartBattle(battleConfigs[currentStageIndex]);  // Start the next battle.
        }
        else
        {
            // No more battles, handle end of game or loop back to the beginning.
            
            currentStageIndex = 0;  // Optional: Reset to the first battle.
        }
    }


    private void StartEnemyTurn()
{
    EnemyAttack();
}


    private void ChangeState(BattleState newState)
    {
        State = newState;
        if (State == BattleState.PlayerTurn)
        {
            EnableAllButtons();
        }
    }

    public void DisableAllButtons()
    {
        /*
        foreach (Button btn in skillButtons)
        {
            btn.interactable = false;
        }
        launchAttacksButton.interactable = false;
        */
    }
   
    public void EnableAllButtons()
    {
       /* foreach (Button btn in skillButtons)
        {
            btn.interactable = true;
        }
        launchAttacksButton.interactable = true;
        */
    }

    public void MoveCursorToTarget()
     {
        // Move the circles to the targets position
        outerCircle.transform.position = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 4f, currentTarget.transform.position.z);


       // innerCircle.transform.position = currentTarget.transform.position;
        //NEEDS OFFSET TO BE OFF SPRITE
            
    }


        public void ExecuteQueuedSkills()
    {
        if (currentTarget != null && (state == BattleState.PlayerTurn))
        {
        DisableAllButtons();
        StartCoroutine(ExecuteAllSkillsCoroutine());
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
       
        int skillsExecuted = 0;
        while(skillQueue.Count > 0)
        {
            Skill skill = skillQueue.Dequeue();
            activePlayer.currentSkill = skill;
            activePlayer.SpendEnergy(skill.energyCost);
            skillsExecuted++;
            yield return StartCoroutine(PlayerAction());
            
        }
    
        skillsExecuted = 0;
        //Make them unable to take a second turn.
        
        
        // Move player back to their original position after all skills executed.
       // yield return StartCoroutine(zoomEffect.ZoomOutEffect());
        yield return activePlayer.ReturnToPosition();
        ChangeState(BattleState.EnemyTurn);
      
      
        }
    }

    

    

    //public ZoomEffect zoomEffect;


    public IEnumerator PlayerAction()
    {
        if ((state == BattleState.PlayerTurn) && currentTarget)
        {
        //  Debug.Log("PlayerAction() being called");

            // Start zoom effect
            //StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position)); 
            activePlayerIndicator.SetActive(false);
            if (activePlayer.currentSkill.requiresMovement && skillsExecuted == 0)
            {
                yield return StartCoroutine(PlayerMoveAndAttackCoroutine());
            }
            else
            {
                yield return StartCoroutine(PlayerAttackCoroutine(null));
            }

        }
    }

    public IEnumerator PlayerMoveAndAttackCoroutine()
    {
        // Only move if the player is not already at the target
        if (activePlayer.transform.position != currentTarget.transform.position)
        {
            yield return activePlayer.MoveToTarget();
            
            
        }

        yield return StartCoroutine(PlayerAttackCoroutine(null));
    }


    public IEnumerator PlayerAttackCoroutine(System.Action successCallback)
    {
        Character targetEnemy = currentTarget.GetComponent<Character>();
        yield return new WaitUntil(() => targetEnemy.isAttacking == false);

        if (activePlayer.currentSkill != null)
        {
            
            yield return activePlayer.currentSkill.Execute(activePlayer, targetEnemy, this);
            
            

            

            CheckBattleEnd();
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
            statusEffectController.ProcessEffects();
            ChangeState(BattleState.PlayerTurn);
            return;
        }

        Character attackingEnemy = enemyTurnQueue.Dequeue();

        //Decide target
         // This sets the enemies target for the turn, need to build out
            //Each enemy can have a preference if needed in their own class
            //For now it will just randomize between the player and companion
            // Use UnityEngine.Random.Range to get a 50/50 chance
            //UnityEngine.Random.Range(min, max) returns either 0 or 1
        attackingEnemy.attackTarget = UnityEngine.Random.Range(0, 2) == 0 ? companion.transform : player.transform;
        if (companion.health <= 0)
        {
            attackingEnemy.attackTarget = player.transform;
        }

        if (player.health <= 0)
        {
            attackingEnemy.attackTarget = companion.transform;
        }

        //Decide whether or not to use the special attack or the normal one
        if (!activePlayer.isAttacking && !attackingEnemy.isAttacking && state == BattleState.EnemyTurn)
        {
            if (attackingEnemy.energy >= attackingEnemy.maxEnergy)
            {
                attackingEnemy.currentSkill = attackingEnemy.specialSkill;
            }
            else 
            {
            attackingEnemy.currentSkill = attackingEnemy.normalSkill;
            }

           
            StartCoroutine(EnemyAttackCoroutine(attackingEnemy));
            
        }
    }
    

    public IEnumerator EnemyAttackCoroutine(Character attackingEnemy)
    {
        yield return new WaitUntil(() => activePlayer.isAttacking == false);
        yield return new WaitForSeconds(1.0f); //Ensures player animation is all done
        

        if (attackingEnemy.currentSkill != null)
        {
            if (attackingEnemy.currentSkill.requiresMovement)
            {
                yield return attackingEnemy.MoveToTarget();
            }

            
            Character targetCharacter = attackingEnemy.attackTarget.GetComponent<Character>();
            yield return attackingEnemy.currentSkill.Execute(attackingEnemy, targetCharacter, this);


           
            if (attackingEnemy.currentSkill.requiresMovement)
            {
                yield return attackingEnemy.ReturnToPosition();
            }
           // Debug.Log("Enemy reurning to position");
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
            
        }

        if (player.health <= 0)
        {
            Debug.Log("You lose ya jabroni");
            SceneManager.LoadScene("CharacterInfoPage");
        }
        else
        {
           // yield return new WaitForSeconds(0.5f);

           // Check if there are more enemies to take their turns
        if (enemyTurnQueue.Count > 0)
        {
            EnemyAttack();  // Next enemy's turn
        }
        else
        {
            
            
            ChangeState(BattleState.PlayerTurn);  // If all enemies had their turns, it's the player's turn next.
         }
        CheckBattleEnd();
        }
    }
    
   
   public CameraShake cameraShake;

    public ColorChanger colorChanger;  // Reference to the ColorLerper script
    
    
    public IEnumerator PlayerActiveTimeEvent(float windowStart, float windowEnd, System.Action<TimingEventResult> callback)
{
    // Enable the timing circles when the event starts
   // outerCircle.SetActive(true);
    //innerCircle.SetActive(true);
    float totalWindowDuration = windowEnd - windowStart;
   // colorChanger.StartColorTransition(totalWindowDuration);
    float timer = 0;
    bool buttonClicked = false;

    // Set the sizes: outer starts bigger and shrinks to size (0,0,0)
    Vector3 outerCircleInitialScale = outerCircle.transform.localScale; // Let's assume this is the size at start.
    Vector3 zeroScale = new Vector3(0, 0, 0); 

    float speedFactor = 1.25f; // Change this value to adjust speed. Higher means faster.


    try
    {
        while (timer < totalWindowDuration)
        {
            
   
            float progress = timer / totalWindowDuration;
            outerCircle.transform.localScale = Vector3.Lerp(outerCircleInitialScale, zeroScale, progress);

            if (Input.GetMouseButtonDown(0))
            {
                buttonClicked = true;
               

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
                result = GetTimingAccuracy(outerCircle.transform.localScale);
                timingFeedbackText.text = result.ToString();
            }
            else
            {
                Debug.Log("Timing Missed!");
                result = TimingEventResult.Miss;
                timingFeedbackText.text = result.ToString();
                skillQueue.Clear();
            }
        }
        else
        {
          
            Debug.Log("No input detected. Missed!");
            result = TimingEventResult.Miss;
            timingFeedbackText.text = result.ToString();
            skillQueue.Clear();
        }

        callback(result);
    }
    finally
    {
        // Reset the scale of the outer circle, ensuring it always happens even if the coroutine is interrupted
        outerCircle.transform.localScale = outerCircleInitialScale;
    }
    // Disable the timing circles when the event ends
    outerCircle.SetActive(false);
    innerCircle.SetActive(false);
}




    
    private TimingEventResult GetTimingAccuracy(Vector3 outerCircleScale)
{
    // Thresholds based on the size of the outer circle
    float perfectThreshold = 0.1f; // This means the circle is very small, almost disappeared
    float goodThreshold = 0.5f; // This means the circle is half its original size

    if (outerCircleScale.x <= perfectThreshold) 
    {
        StartCoroutine(cameraShake.Shake());
        return TimingEventResult.Perfect;
    }
    else if (outerCircleScale.x <= goodThreshold)
    {
        StartCoroutine(cameraShake.Shake());
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
        
        colorChanger.StartColorTransition(totalHoldDuration);

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
    PlayerData.Instance.SavePlayerData();
    


    TextMeshProUGUI expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
    expGainedTextComponent.text = totalExp.ToString();

    TextMeshProUGUI goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
    goldGainedTextComponent.text = totalGold.ToString();

}



    public bool isSkillSelected = false;  // New variable


private void Update()
{
    if (!player.isAttacking || !companion.isAttacking)
    {
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            // If the hit object is an enemy
            if (hit.collider.CompareTag("Enemy")) // && isSkillSelected)
            {
                currentTarget = hit.collider.gameObject;
                MoveCursorToTarget();
                outerCircle.SetActive(true);
                activePlayer.attackTarget = currentTarget.transform;

                // Execute the queued skill here since an enemy is tapped after selecting a skill
                if ((state == BattleState.PlayerTurn) && isSkillSelected)
                {
                    ExecuteQueuedSkills();
                }
                isSkillSelected = false;  // Reset the flag
            }
            // If the hit object is a player and has not gone yet
            else if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Companion"))
            {
                Character clickedCharacter = hit.collider.GetComponent<Character>();
                
                if (clickedCharacter != null && clickedCharacter.hasNotGone)
                {
                    activePlayer = clickedCharacter;
                    Debug.Log("active player switched");// Move the circles to the targets position
                    activePlayerIndicator.transform.position = new Vector3(activePlayer.transform.position.x, activePlayer.transform.position.y + 2f, activePlayer.transform.position.z);
                    activePlayerIndicator.SetActive(true);

                }
            }
        }
    }
    }
    ChangeColorAfterTurnTaken();
}


    public void ChangeColorAfterTurnTaken()
    {
        if (!activePlayer.hasNotGone) 
        {
            activePlayer.GetComponent<SpriteRenderer>().color = Color.gray;
        }
        else 
        {
            activePlayer.GetComponent<SpriteRenderer>().color = Color.white;
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
    new WaitForSeconds(3.0f);
    if (allEnemiesDefeated)
    {
        EndOfBattleRewards(enemies);
        GameManager.Instance.UnlockNextStage(GameManager.Instance.currentStageID); //Needs to move
        endOfBattlePanel.SetActive(true);
        
    }
}




}