using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

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
    public List<Character> playerParty = new List<Character>();
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
    public GameObject endOfBattleLossPanel;

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
                    Debug.Log("Active player from Player1" + activePlayer);
                    break;
                case BattleState.EnemyTurn:
                    Debug.Log("Enemy Turn Started!");
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
    endOfBattlePanel.SetActive(false);
    for (int i = 0; i < config.maxEnemiesToSpawn; i++)
    {
        Character spawnedEnemy = enemySpawnController.SpawnEnemiesFromPool(
            config.poolName,
            1, // Spawn one enemy
            enemySpawnPoints[i], // Pass the spawn point
            healthBars[i], // Pass the health bar slider
            energyBars[i], // Pass the energy bar slider
            i // Pass the index for health text assignment
        );

        if (spawnedEnemy != null)
        {
            enemies.Add(spawnedEnemy);
        }
    }
    InitializeTurnOrder();
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


   


    private void ChangeState(BattleState newState)
    {
        State = newState;
        if (State == BattleState.PlayerTurn)
        {

            
            EnableAllButtons();
        }
    }

    public List<Character> turnOrderList = new List<Character>();
    public void InitializeTurnOrder()
    {
            // Clear the previous turn order list
        turnOrderList.Clear();

        // Add player and companion to the turn order list if they are not null
        if (player != null) turnOrderList.Add(player);
        if (companion != null) turnOrderList.Add(companion);

        // Add all enemies to the turn order list
        turnOrderList.AddRange(enemies);

            // Sort the list by speed in descending order (highest speed first)
        turnOrderList = turnOrderList.OrderByDescending(character => character.speed).ToList();

        StartTurn();

    }

    public void StartTurn()
{
    if (turnOrderList.Count > 0)
    {
        var nextCharacter = turnOrderList[0];
        if (nextCharacter.health <= 0) //check to make sure the character is still alive.
        {
            turnOrderList.Remove(nextCharacter);
            ExecuteTurn(turnOrderList[0]);
        }
    else
    {
        ExecuteTurn(nextCharacter);
    }
    }
    else
    {
        InitializeTurnOrder();
    }
}

private void ExecuteTurn(Character character)
{
    if (character == player || character == companion)
    {
        activePlayer = character;
        ChangeState(BattleState.PlayerTurn);
    }
    else // Assuming the character is an enemy
    {
        ChangeState(BattleState.EnemyTurn);
        EnemyAttack(character); // Pass the current enemy character
    }
}


public void EndTurn()
{
    turnOrderList.RemoveAt(0); // Remove the character from the list after their turn
    Debug.Log("ENDING THE TURN");
    if (turnOrderList.Count == 0)
    {
        InitializeTurnOrder();
    }
    else
    {
        StartTurn(); // Proceed to the next character's turn
    }
}

// This method should be called when the player has finished their turn




    

    public void DisableAllButtons()
    {
        /*
        foreach (Button btn in skillButtons)
        {
            btn.interactable = false;
        }
        launchAttacksButton.interactable = false;
        */
      //  enemyUIPanel.SetActive(false);
    }
   public GameObject enemyUIPanel;
    public void EnableAllButtons()
    {
       /* foreach (Button btn in skillButtons)
        {
            btn.interactable = true;
        }
        launchAttacksButton.interactable = true;
        */
        //enemyUIPanel.SetActive(true);
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
        StartCoroutine(zoomEffect.ZoomOutEffect());
        yield return activePlayer.ReturnToPosition();
        yield return new WaitUntil(() => activePlayer.isMoving == false);
        EndTurn();

        
      
      
        }
    }

    

    

    public ZoomEffect zoomEffect;


    public IEnumerator PlayerAction()
    {
        if ((state == BattleState.PlayerTurn) && currentTarget)
        {
        //  Debug.Log("PlayerAction() being called");

             
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
        // Start zoom effect
            StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position));
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
    public Character attackingEnemy;
    
    public void EnemyAttack(Character currentEnemy)
{
    // Ensure that we are in the enemy turn and attackingEnemy is set
    if (state != BattleState.EnemyTurn || currentEnemy == null)
    {
        Debug.LogError("It's not an enemy's turn or attackingEnemy is null");
        return;
    }

    // Decide whether or not to use the special attack or the normal one
    if (!currentEnemy.isAttacking)
    {
        currentEnemy.currentSkill = (currentEnemy.energy >= currentEnemy.maxEnergy) ? 
            currentEnemy.specialSkill : currentEnemy.normalSkill;

        // Set the target for the attacking enemy
        currentEnemy.attackTarget = player.transform; //UnityEngine.Random.Range(0, 2) == 0 ? companion.transform : player.transform;

        // Start the enemy attack coroutine
        StartCoroutine(EnemyAttackCoroutine(currentEnemy));
    }
}


    

    public IEnumerator EnemyAttackCoroutine(Character currentEnemy)
    {
        yield return new WaitUntil(() => activePlayer.isAttacking == false);
        yield return new WaitForSeconds(1.0f); //Ensures player animation is all done
        

        if (currentEnemy.currentSkill != null)
        {
            if (currentEnemy.currentSkill.requiresMovement)
            {
                yield return currentEnemy.MoveToTarget();
            }

            
            Character targetCharacter = currentEnemy.attackTarget.GetComponent<Character>();
            yield return currentEnemy.currentSkill.Execute(currentEnemy, targetCharacter, this);


           
            if (currentEnemy.currentSkill.requiresMovement)
            {
                yield return currentEnemy.ReturnToPosition();
            }
           // Debug.Log("Enemy reurning to position");
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
            
        }

        
          
        EndTurn();
        CheckBattleEnd();
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
    outerCircle.SetActive(true);


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
    if (!player.isAttacking && !companion.isAttacking)
    {
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && (!player.isAttacking && !companion.isAttacking))
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
                    //activePlayer = clickedCharacter;
                    Debug.Log("active player switched");// Move the circles to the targets position
                    activePlayerIndicator.transform.position = new Vector3(activePlayer.transform.position.x, activePlayer.transform.position.y + 2f, activePlayer.transform.position.z);
                   

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
    bool allAlliesDefeated = player.health <= 0 && companion.health <= 0;

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
        Debug.Log("Is this running");
        Stage completedStage = GameManager.Instance.CurrentBattleConfig.stage;
        GameManager.Instance.UnlockConnectedStages(completedStage);
        endOfBattlePanel.SetActive(true);
    }
    else if (allAlliesDefeated)
    {
        endOfBattleLossPanel.SetActive(true);
    }
}




}