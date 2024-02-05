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
    public Character companion1;
    public Character companion2;
    public Character activePlayer;
    public PlayerData playerCharacterData;
    //public Player playerClassReference;
    public List<Character> enemies = new List<Character>();
    public List<Character> playerParty = new List<Character>();
    public GameObject activePlayerIndicator;
    public GameObject heroSelectionPanel; // Assign in inspector
    public GameObject battleStartButton;
    public GameObject heroButtonPrefab;   // Assign in inspector

    
    
    
    

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
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private GameObject popupStartPoint; // Reference to the starting point of the timing feedback text found in PlayerActiveEvent

    




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
     public CompanionSpawnController companionSpawnController; // Assign in inspector



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
        companionSpawnController.CreateHeroSelectionUI();
    }
    public bool isBattleStarted = false;
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
         //   StartBattle(config);
        }
        else
        {
            Debug.LogError("No battle configuration found.");
        }
    

    Debug.Log("Hero Selection Panel: " + heroSelectionPanel);
Debug.Log("Hero Button Prefab: " + heroButtonPrefab);
Debug.Log("GameManager Instance: " + GameManager.Instance);
Debug.Log("Companions: " + GameManager.Instance.companions);

    
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
     if (isBattleStarted) return; // Prevent starting the battle multiple times
        battleStartButton.SetActive(false);
        activePlayer = companion1; //Default
        isBattleStarted = true;
        // Existing logic to start the battle
    endOfBattlePanel.SetActive(false);
    heroSelectionPanel.SetActive(false);
    companionSkillsPanel.SetActive(true);
    for (int i = 0; i < config.maxEnemiesToSpawn; i++)
    {
        Debug.Log(config.levelOfEnemies + "config levelOfEnemies");
        Character spawnedEnemy = enemySpawnController.SpawnEnemiesFromPool(
            config.poolName,
            1, // Spawn one enemy
            enemySpawnPoints[i], // Pass the spawn point
            healthBars[i], // Pass the health bar slider
            energyBars[i], // Pass the energy bar slider
            i, // Pass the index for health text assignment
            config.levelOfEnemies //For level assignment
        );

        if (spawnedEnemy != null)
        {
            enemies.Add(spawnedEnemy);
            
        }
    }
    RestoreHealthAndEnergy();
    InitializeTurnOrder();
}

    public void RestoreHealthAndEnergy()
    {
        companion1.energy = companion1.maxEnergy;
        companion1.health = companion1.maxHealth;

       // companion2.energy = companion2.maxEnergy;
       // companion2.health = companion2.maxHealth;
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
    turnOrderList.Clear();

    if (companion1 != null) turnOrderList.Add(companion1);
    if (companion2 != null) turnOrderList.Add(companion2);

    foreach (var enemy in enemies)
    {
        turnOrderList.Add(enemy);
        enemy.attackTarget = SelectTargetForEnemy(); // Assign a target to each enemy
    }

    turnOrderList = turnOrderList.OrderByDescending(character => character.speed).ToList();
    StartTurn();
}


    public void StartTurn()
{
    // Remove all characters with health less than or equal to zero
    turnOrderList.RemoveAll(character => character.health <= 0);

    // Now check if there are characters left to take a turn
    if (turnOrderList.Count > 0)
    {
        var nextCharacter = turnOrderList[0];
        ExecuteTurn(nextCharacter);
    }
    else
    {
        // If no characters are left, re-initialize the turn order
        InitializeTurnOrder();
    }
}

private void ExecuteTurn(Character character)
{
    if (character == companion1 || character == companion2)
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
        companionSkillsPanel.SetActive(false);
        SkillsPanel.SetActive(false);
        enemyUIPanel.SetActive(false);
       // heroUIPanels.SetActive(false);
    }

   public GameObject enemyUIPanel;
   public GameObject heroUIPanels;
   public GameObject companionSkillsPanel;
   public GameObject SkillsPanel;

    public void EnableAllButtons()
    {
        
        companionSkillsPanel.SetActive(true);
        SkillsPanel.SetActive(true);
        enemyUIPanel.SetActive(true);
       // heroUIPanels.SetActive(true);
        
    }

    public void MoveCursorToTarget()
     {
        // Move the circles to the targets position
        if (currentTarget != null){

        outerCircle.transform.position = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 4f, currentTarget.transform.position.z);
        }

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
            enemyUIPanel.SetActive(false);
            
        // Only move if the player is not already at the target
        if (activePlayer.transform.position != currentTarget.transform.position)
        {
            activePlayer.originalPosition = activePlayer.transform.position;
            Debug.Log("OP set to " + activePlayer.originalPosition);
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
        if (activePlayer.currentSkill.requiresMovement == true)
        {
        // Move player back to their original position after all skills executed.
        StartCoroutine(zoomEffect.ZoomOutEffect());
        enemyUIPanel.SetActive(true);
       
        yield return activePlayer.ReturnToPosition();
        }
    }

    private Queue<Character> enemyTurnQueue = new Queue<Character>();
    public Character attackingEnemy;
    
    public Transform GetWeightedRandomTarget(List<Transform> targets, List<float> weights)
{
    float totalWeight = weights.Sum();
    float randomNumber = UnityEngine.Random.Range(0, totalWeight);
    float cumulativeWeight = 0;

    for (int i = 0; i < targets.Count; i++)
    {
        cumulativeWeight += weights[i];
        if (randomNumber <= cumulativeWeight)
        {
            return targets[i];
        }
    }

    return null; // In case no target is selected, which should not happen
}
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

        
        StartCoroutine(EnemyAttackCoroutine(currentEnemy));
    }
}

    public Transform SelectTargetForEnemy()
{
    List<Transform> potentialTargets = new List<Transform>();
    List<float> targetWeights = new List<float>();

    // Add players to potential targets if they are alive
    if (companion1 != null && companion1.health > 0)
    {
        potentialTargets.Add(companion1.transform);
        targetWeights.Add(GameManager.Instance.probabilityCompanion1);
    }
    if (companion2 != null && companion2.health > 0)
    {
        potentialTargets.Add(companion2.transform);
        targetWeights.Add(GameManager.Instance.probabilityCompanion2);
    }

    if (potentialTargets.Count == 0)
    {
        Debug.LogError("No valid targets available.");
        return null;
    }

    // Use weighted random selection to choose a target
    return GetWeightedRandomTarget(potentialTargets, targetWeights);
}

   
    

    public IEnumerator EnemyAttackCoroutine(Character currentEnemy)
{
    yield return new WaitUntil(() => activePlayer.isAttacking == false);
    yield return new WaitForSeconds(1.0f); //Ensures player animation is all done

    // Select the target for the enemy and assign it
    Transform enemyTargetTransform = SelectTargetForEnemy();
    currentEnemy.attackTarget = enemyTargetTransform;
    currentTarget = enemyTargetTransform.gameObject; // Update currentTarget to the selected target

    if (currentEnemy.currentSkill != null)
    {
        if (currentEnemy.currentSkill.requiresMovement)
        {
            yield return currentEnemy.MoveToTarget();
        }

        Character targetCharacter = enemyTargetTransform.GetComponent<Character>();

        yield return currentEnemy.currentSkill.Execute(currentEnemy, targetCharacter, this);

        if (currentEnemy.currentSkill.requiresMovement)
        {
            yield return currentEnemy.ReturnToPosition();
        }
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
    
    private void ShowTimingResult(string message)
{
    if (currentTarget == null)
    {
        Debug.LogError("No current target set for timing result popup.");
      
    }
    
    // Position the popup above the current target
    Vector3 targetPosition = currentTarget.transform.position;
    float yOffset = 3.0f; // Adjust this value as needed for the correct height
    Vector3 popupPosition = new Vector3(targetPosition.x, targetPosition.y + yOffset, targetPosition.z);

    // Assuming that the DamagePopup prefab is correctly located at Resources/Prefab/DamagePopup
    GameObject damagePopupPrefab = Resources.Load<GameObject>("Prefab/UI Elements/TimingResultPopup");
    Transform canvasTransform = GameObject.Find("EndOfBattleRewardsCanvas").transform; // Replace with your actual Canvas name if different

    if (damagePopupPrefab != null)
    {
        GameObject damagePopupInstance = Instantiate(damagePopupPrefab, popupPosition, Quaternion.identity, canvasTransform);
        DamagePopup damagePopupScript = damagePopupInstance.GetComponent<DamagePopup>();
        damagePopupScript.SetupTimingEventResult(message); // Call the method to set up timing result text
    }
    else
    {
        Debug.LogError("Failed to load DamagePopup prefab for timing result.");
    }
}





        
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
        ShowTimingResult(result.ToString());
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

        ShowTimingResult(result.ToString());
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
    Debug.Log("Running end of battle rewards +" + totalExp + " +" + totalGold);
    GameManager.Instance.GainExp(totalExp);
    PlayerData.Instance.gold += totalGold;
    PlayerData.Instance.SavePlayerData();
    GameManager.Instance.SaveAllCompanionData();
    


    TextMeshProUGUI expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
    expGainedTextComponent.text = totalExp.ToString();

    TextMeshProUGUI goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
    goldGainedTextComponent.text = totalGold.ToString();

}



    public bool isSkillSelected = false;  // New variable



private void Update()
{
    if (isBattleStarted){
    if (!companion1.isAttacking && !companion2.isAttacking)
    {
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && (!companion1.isAttacking && !companion2.isAttacking))
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
    }
    //ChangeColorAfterTurnTaken();
}

    


//NOT BEING USED;Would need to implement for enemies and allies, maybe later due to changing battle
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
    bool allEnemiesDefeated = false;
    bool allAlliesDefeated = companion1.health <= 0 && companion2.health <= 0;

    foreach (var enemy in enemies)
    {
        if (enemy.health > 0)
        {
            allEnemiesDefeated = false;
            break;
        }
        else 
        {
            allEnemiesDefeated = true;
        }
       
    }
    
    if (allEnemiesDefeated)
    {
        EndOfBattleRewards(enemies);
        Debug.Log("Is this running");
       // Stage completedStage = GameManager.Instance.CurrentBattleConfig.stage;
       // GameManager.Instance.UnlockConnectedStages(completedStage);
        endOfBattlePanel.SetActive(true);
    }
    else if (allAlliesDefeated)
    {
        endOfBattleLossPanel.SetActive(true);
    }
}




}