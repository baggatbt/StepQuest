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
    public List<Character> enemies = new List<Character>();
    public List<Character> playerParty = new List<Character>();
    public GameObject activePlayerIndicator;
    public GameObject heroSelectionPanel;
    public GameObject battleStartButton;
    public GameObject heroButtonPrefab;
    public GameObject skillDescriptionPanel;
    public GameObject outerCircle;
    public GameObject innerCircle;
    public GameObject ExpGainedText;
    public GameObject GoldGainedText;
    public GameObject endOfBattlePanel;
    public GameObject endOfBattleLossPanel;
    public GameObject damagePopupPrefab;
    public GameObject popupStartPoint;
    public HoldReleaseSlider holdReleaseSlider;
    public Slider[] healthBars;
    public Slider[] energyBars;
    public GameObject[] enemyHealthUI;
    public Skill requestedSkill;
    public Queue<Skill> skillQueue = new Queue<Skill>();
    public Button[] skillButtons;
    public Button launchAttacksButton;
    public StatusEffectController statusEffectController;
    public TextMeshProUGUI timingFeedbackText;
    public EnemySpawnController enemySpawnController;
    public CompanionSpawnController companionSpawnController;
    public Button nextBattleButton;
    public Transform[] enemySpawnPoints;
    public BattleConfig currentBattleConfig;
    public BattleConfig[] battleConfigs;
    public GameObject currentTarget;

    private Vector3 outerCircleInitialScale;
    private Vector3 innerCircleInitialScale;
    private TextMeshProUGUI expGainedTextComponent;
    private TextMeshProUGUI goldGainedTextComponent;
    private Camera mainCamera;
    private BattleState state;
    private int currentStageIndex = 0;
    public bool isBattleStarted;
    private bool battleLost = false;

    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
        BattleLost
    }

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
                case BattleState.BattleLost:
                    Debug.Log("Battle lost");
                    break;
            }
        }
    }

    private void Awake()
    {
        // Initialize any necessary components here
         companionSpawnController.CreateHeroSelectionUI();
    }

    private void Start()
    {
        outerCircleInitialScale = outerCircle.transform.localScale;
        innerCircleInitialScale = innerCircle.transform.localScale;

        outerCircle.SetActive(false);
        activePlayerIndicator.SetActive(false);
        innerCircle.SetActive(false);

        GameManager.Instance.LoadCurrentParty();
       // SetupBattle(GameManager.Instance.currentParty);
         RestoreHealthAndEnergy();
        expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
        goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
        mainCamera = Camera.main;

        BattleConfig config = GameManager.Instance.CurrentBattleConfig;
        if (config != null)
        {
            
            Debug.Log("StartBattle is being called");
        }
        else
        {
            Debug.LogError("No battle configuration found.");
        }

        
        for (int i = 0; i < config.maxEnemiesToSpawn; i++)
        {
            // Activate the spawn point if it is inactive
            if (!enemySpawnPoints[i].gameObject.activeSelf)
            {
                enemySpawnPoints[i].gameObject.SetActive(true);
            }

            Character spawnedEnemy = enemySpawnController.SpawnEnemiesFromPool(
                config.poolName,
                1,
                enemySpawnPoints[i],
                healthBars[i],
                energyBars[i],
                i,
                config.levelOfEnemies,
                enemyHealthUI[i]
            );

            if (spawnedEnemy != null)
            {
                enemies.Add(spawnedEnemy);
            }
        }
       
       // StartBattle(config);
    }

    public GameObject knightPrefab;
    

    /*
   public void SetupBattle(List<Companion> currentParty)
    {
        int companionIndex = 0; // Initialize the index
        foreach (var companion in currentParty)
        {
            Character instantiatedCompanion = InstantiateCompanion(companion.heroID);
            if (instantiatedCompanion is Companion instantiatedCompanionAsCompanion)
            {
                instantiatedCompanionAsCompanion.InitializeSkillsBasedOnLevel();
                companionSpawnController.SetupCompanion(instantiatedCompanionAsCompanion, companionIndex); // Pass the index here
                companionSpawnController.EnableHeroUI(instantiatedCompanionAsCompanion);
                instantiatedCompanionAsCompanion.isSelected = true;
                activePlayer = instantiatedCompanion;
                companionIndex++; // Increment the index
                playerParty.Add(instantiatedCompanion);
            }
        }
    }
    */

    public void StartBattle()
    {
         if (playerParty.Count > 0) {
     if (isBattleStarted) return; // Prevent starting the battle multiple times
        //TODO To be moved outside of the dungeon entry call DeductStaminaFromParticipants();
        
        battleStartButton.SetActive(false);
        
        isBattleStarted = true;
        // Existing logic to start the battle
    endOfBattlePanel.SetActive(false);
    heroSelectionPanel.SetActive(false);

            if (playerParty.Count > 0)
            {
                companion1 = playerParty[0];
                Debug.Log("Companion1 was assigned");
            }
            if (playerParty.Count > 1)
            {
                companion2 = playerParty[1];
                Debug.Log("Companion2 was assigned");
            }

            
        }
        
        activePlayer = companion1; //Default
        InitializeTurnOrder();
    }


    public void RestoreHealthAndEnergy()
    {
        foreach (var companion in playerParty.OfType<Companion>())
        {
            companion.energy = companion.maxEnergy;
            companion.health = companion.maxHealth;
        }
    }

    

    public IEnumerator TimeStop(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private void ChangeState(BattleState newState)
    {
        CheckBattleEnd();
        turnOrderList.RemoveAll(character => character.health <= 0);
        State = newState;
        if (State == BattleState.PlayerTurn)
        {
            StartCoroutine(EnableAllButtons());
        }
    }

    public List<Character> turnOrderList = new List<Character>();

    public GameObject turnOrderBarPanel;
    public GameObject iconPrefab; //Has an image attached so it can be used for enemy or hero


    public void InitializeTurnOrder()
    {
        if (!battleLost)
        {
            turnOrderList.Clear();
            
            // Clear existing icons from the TurnOrderBarPanel
            foreach (Transform child in turnOrderBarPanel.transform)
            {
                Destroy(child.gameObject);
            }

            if (companion1 != null) turnOrderList.Add(companion1);
            if (companion2 != null) turnOrderList.Add(companion2);

            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    turnOrderList.Add(enemy);
                    enemy.attackTarget = SelectTargetForEnemy();
                    Debug.Log("Enemy added to turn order: " + enemy.name);
                }
            }

            // Order by speed descending
            turnOrderList = turnOrderList.OrderByDescending(character => character.speed).ToList();

            // Create icons for each character in the turn order list
            foreach (var character in turnOrderList)
            {
                GameObject icon = Instantiate(iconPrefab, turnOrderBarPanel.transform);
                Image iconImage = icon.GetComponent<Image>();
                
                if (character is Companion)
                {
                    iconImage.sprite = (character as Companion).heroIcon;
                }
                else if (character is Enemy)
                {
                    iconImage.sprite = (character as Enemy).enemyIcon;
                }
            }

            StartTurn();
        }
        else
        {
            Debug.Log("The battle is over");
        }
    }

    public void StartTurn()
{
    
    if (battleLost != true)
    {
    // Now check if there are characters left to take a turn
    if (turnOrderList.Count > 0)
    {
        turnOrderList.RemoveAll(character => character.health <= 0);
        var nextCharacter = turnOrderList[0];
        ExecuteTurn(nextCharacter);
    }
    else
    {
        // If no characters are left, re-initialize the turn order
        InitializeTurnOrder();
    }
    }
}

    private void ExecuteTurn(Character character)
    {
        if (character == companion1 || character == companion2)
        {
            activePlayer = character;
            ChangeState(BattleState.PlayerTurn);
        }
        else
        {
            ChangeState(BattleState.EnemyTurn);
            EnemyAttack(character);
        }
    }

    public void EndTurn()
    {
        turnOrderList.RemoveAt(0);
        turnOrderList.RemoveAll(character => character.health <= 0);
        if (turnOrderList.Count == 0)
        {
            statusEffectController.ProcessEffects();
            InitializeTurnOrder();
        }
        else
        {
            StartTurn();
        }
    }

    private void DisableSkillSelection()
    {
        companionSkillsPanel.SetActive(false);
        SkillsPanel.SetActive(false);
    }

    public void DisableAllButtons()
    {
        companionSkillsPanel.SetActive(false);
        SkillsPanel.SetActive(false);
        enemyUIPanel.SetActive(false);
        heroUIPanels.SetActive(false);
    }

    public GameObject enemyUIPanel;
    public GameObject heroUIPanels;
    public GameObject companionSkillsPanel;
    public GameObject SkillsPanel;

    public IEnumerator EnableAllButtons()
    {
        yield return new WaitForSeconds(0.25f);
        if (state == BattleState.PlayerTurn)
        {
            companionSkillsPanel.SetActive(true);
        }
        SkillsPanel.SetActive(true);
        enemyUIPanel.SetActive(true);
        heroUIPanels.SetActive(true);
    }

    public void MoveCursorToTarget()
    {
        if (currentTarget != null)
        {
            outerCircle.transform.position = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 4.0f, currentTarget.transform.position.z);
        }
    }

    public Transform radialMenuPanelCenterPoint;
    public void MoveRadialMenuToActivePlayer()
    {
        radialMenuPanelCenterPoint.transform.position = new Vector3(activePlayer.transform.position.x, activePlayer.transform.position.y + 3f, activePlayer.transform.position.z);
    }

    public void ExecuteQueuedSkills()
    {
        if (currentTarget != null && (state == BattleState.PlayerTurn))
    {
        Debug.Log("Executing queued skills on target: " + currentTarget.transform.position);
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
            skillsExecuted = 0;
            while (skillQueue.Count > 0)
            {
                
                Skill skill = skillQueue.Dequeue();
                activePlayer.currentSkill = skill;
                activePlayer.SpendEnergy(skill.energyCost);
                skillsExecuted++;
                yield return StartCoroutine(PlayerAction());
            }

            skillsExecuted = 0;
            turnOrderList.RemoveAll(character => character.health <= 0);
            EndTurn();
            yield return new WaitUntil(() => activePlayer.isMoving == false);
        }
    }

    public ZoomEffect zoomEffect;

    public IEnumerator PlayerAction()
{
    if ((state == BattleState.PlayerTurn) && currentTarget)
    {
        activePlayerIndicator.SetActive(false);
        if (activePlayer.currentSkill.requiresMovement)
        {
            Debug.Log("Player moving to attack target: " + currentTarget.transform.position);
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
        if (!activePlayer.currentSkill.noZoom)
        {
            StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position));
        }
            enemyUIPanel.SetActive(false);
            
        // Only move if the player is not already at the target
        if (activePlayer.transform.position != currentTarget.transform.position)
        {
            activePlayer.originalPosition = activePlayer.transform.position;
            Debug.Log("OP set to " + activePlayer.originalPosition);
            yield return activePlayer.MoveToTarget();
            
            
        }
//ARROW DOESNT USE MOVE AND ATTACK INVESTIGATE THERE
        yield return StartCoroutine(PlayerAttackCoroutine(null));
    }

    public IEnumerator PlayerAttackCoroutine(System.Action successCallback)
    {
        
        Character targetEnemy = currentTarget.GetComponent<Character>();
        yield return new WaitUntil(() => targetEnemy.isAttacking == false);

        if (activePlayer.currentSkill != null)
        {
            
            yield return activePlayer.currentSkill.Execute(activePlayer, targetEnemy, this);
            
           // CheckBattleEnd();
        }
        if (activePlayer.currentSkill.requiresMovement == true)
        {
        // Move player back to their original position after all skills executed.

        if (!activePlayer.currentSkill.noZoom)
        {
            StartCoroutine(zoomEffect.ZoomOutEffect());
        }
        
        StartCoroutine(EnableAllButtons());

       
        yield return activePlayer.ReturnToPosition();
        }
        if(activePlayer.currentSkill.requiresMovement == false)
        {
            yield return new WaitForSeconds(0.5f); //Ensures fade is all done
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

        return null;
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


    public void EnemyAttack(Character currentEnemy)
    {
        if (state != BattleState.EnemyTurn || currentEnemy == null)
        {
            Debug.LogError("It's not an enemy's turn or attackingEnemy is null");
            return;
        }

        if (!currentEnemy.isAttacking && !battleLost)
        {
            currentEnemy.currentSkill = currentEnemy.normalSkill;
            StartCoroutine(EnemyAttackCoroutine(currentEnemy));
        }
    }

    public Transform SelectTargetForEnemy()
    {
        List<Transform> potentialTargets = new List<Transform>();
        List<float> targetWeights = new List<float>();

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
            Debug.Log("No valid targets available.");
            return null;
        }

        return GetWeightedRandomTarget(potentialTargets, targetWeights);
    }

    public IEnumerator EnemyAttackCoroutine(Character currentEnemy)
    {
        yield return new WaitUntil(() => activePlayer.isAttacking == false);
        yield return new WaitForSeconds(1.0f);

        Transform enemyTargetTransform = SelectTargetForEnemy();
        currentEnemy.attackTarget = enemyTargetTransform;
        currentTarget = enemyTargetTransform.gameObject;

        if (currentEnemy.currentSkill != null)
        {
            if (currentEnemy.currentSkill.requiresMovement)
            {
                StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position));
                enemyUIPanel.SetActive(false);
                DisableAllButtons();
                yield return currentEnemy.MoveToTarget();
            }

            Character targetCharacter = enemyTargetTransform.GetComponent<Character>();
            yield return currentEnemy.currentSkill.Execute(currentEnemy, targetCharacter, this);

            if (currentEnemy.currentSkill.requiresMovement)
            {
                StartCoroutine(zoomEffect.ZoomOutEffect());
                yield return currentEnemy.ReturnToPosition();
                StartCoroutine(EnableAllButtons());
            }
        }
        else
        {
            Debug.Log("Standard Attack Performed - This should not happen");
        }

        EndTurn();
    }

    public CameraShake cameraShake;

    private void ShowTimingResult(string message)
    {
        if (currentTarget == null)
        {
            Debug.LogError("No current target set for timing result popup.");
        }

        Vector3 targetPosition = currentTarget.transform.position;
        float yOffset = 3.0f;
        Vector3 popupPosition = new Vector3(targetPosition.x, targetPosition.y + yOffset, targetPosition.z);

        GameObject damagePopupPrefab = Resources.Load<GameObject>("Prefab/UI Elements/TimingResultPopup");
        Transform canvasTransform = GameObject.Find("EndOfBattleRewardsCanvas").transform;

        if (damagePopupPrefab != null)
        {
            GameObject damagePopupInstance = Instantiate(damagePopupPrefab, popupPosition, Quaternion.identity, canvasTransform);
            DamagePopup damagePopupScript = damagePopupInstance.GetComponent<DamagePopup>();
            damagePopupScript.SetupTimingEventResult(message);
        }
        else
        {
            Debug.LogError("Failed to load DamagePopup prefab for timing result.");
        }
    }

    public IEnumerator PlayerActiveTimeEvent(float windowStart, float windowEnd, System.Action<TimingEventResult> callback)
    {
        float totalWindowDuration = windowEnd - windowStart;
        float timer = 0;
        bool buttonClicked = false;
        float speedFactor = 1.0f;

        Vector3 targetScale = outerCircle.transform.localScale;
        Vector3 initialScale = new Vector3(0, 0, 0);
        innerCircle.transform.localScale = initialScale;

        try
        {
            while (timer < totalWindowDuration)
            {
                float progress = timer / totalWindowDuration;
                innerCircle.transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

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
                result = GetTimingAccuracy(innerCircle.transform.localScale, targetScale, timer, windowStart, windowEnd);
            }
            else
            {
                Debug.Log("No input detected. Missed!");
                result = TimingEventResult.Late;
            }

            callback(result);
            ShowTimingResult(result.ToString());
        }
        finally
        {
            outerCircle.SetActive(false);
            innerCircle.SetActive(false);
        }
    }

    private TimingEventResult GetTimingAccuracy(Vector3 innerCircleScale, Vector3 outerCircleScale, float timer, float windowStart, float windowEnd)
    {
        float perfectTiming = (windowEnd + windowStart) / 2;
        float accuracy = Mathf.Abs(timer - perfectTiming);
        float windowSize = windowEnd - windowStart;
        float perfectThreshold = windowSize * 0.3f;

        if (accuracy <= perfectThreshold)
        {
            StartCoroutine(cameraShake.Shake(0.5f));
            return TimingEventResult.Good;
        }
        else if (timer < perfectTiming)
        {
            StartCoroutine(cameraShake.Shake(0.25f));
            return TimingEventResult.Early;
        }
        else
        {
            StartCoroutine(cameraShake.Shake(0.25f));
            return TimingEventResult.Late;
        }
    }

    public IEnumerator PlayerHoldReleaseTimeEvent(float holdStart, float holdEnd, Action<TimingEventResult> callback)
    {
        float totalHoldDuration = holdEnd - holdStart;
        float holdTimer = 0;

        holdReleaseSlider.ResetSlider();
        holdReleaseSlider.gameObject.SetActive(true);

        while (holdTimer < totalHoldDuration)
        {
            if (Input.GetMouseButton(0))
            {
                holdTimer += Time.deltaTime;
                holdReleaseSlider.UpdateSlider(holdTimer / totalHoldDuration);
            }

            if (Input.GetMouseButtonUp(0))
            {
                holdReleaseSlider.gameObject.SetActive(false);
                break;
            }

            yield return null;
        }

        TimingEventResult result;
        float perfectThreshold = totalHoldDuration * 0.1f;

        if (Mathf.Abs(holdTimer - totalHoldDuration) <= perfectThreshold)
        {
            result = TimingEventResult.Good;
        }
        else if (holdTimer < totalHoldDuration)
        {
            result = TimingEventResult.Early;
        }
        else
        {
            result = TimingEventResult.Late;
        }

        ShowTimingResult(result.ToString());
        callback(result);
        holdReleaseSlider.ResetSlider();
    }

    public BattleState GetState()
    {
        return state;
    }

    public Companion companion;
    public Slider[] expSliders;
    public TextMeshProUGUI[] expTexts;
    public TextMeshProUGUI[] heroIDTexts;

    private void DisplayExpToLevel(List<Character> playerParty)
    {
        foreach (var slider in expSliders) slider.gameObject.SetActive(false);
        foreach (var text in expTexts) text.gameObject.SetActive(false);
        foreach (var text in heroIDTexts) text.gameObject.SetActive(false);

        for (int i = 0; i < playerParty.Count; i++)
        {
            if (playerParty[i] is Companion companion)
            {
                int expToLevel = companion.ExpToNextLevel(companion.heroLevel);

                if (i < expSliders.Length && i < expTexts.Length)
                {
                    expSliders[i].maxValue = companion.ExpToNextLevel(companion.heroLevel);
                    expSliders[i].value = companion.heroExp;
                    expTexts[i].text = "Exp to level: " + (expToLevel - companion.heroExp).ToString();
                    heroIDTexts[i].text = companion.heroID;

                    expSliders[i].gameObject.SetActive(true);
                    expTexts[i].gameObject.SetActive(true);
                    heroIDTexts[i].gameObject.SetActive(true);
                }
                Debug.Log(expTexts[i].text);
            }
        }
    }

    private float CalculateSliderValue(int currentExp, int expToNextLevel)
    {
        return (float)currentExp / expToNextLevel;
    }

    public bool isSkillSelected = false;

    private void Update()
{
    
    if (isBattleStarted){
        MoveRadialMenuToActivePlayer();
    if (!playerParty.Any(character => character.isAttacking) && !enemies.Any(character => character.isAttacking))
    {
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && !playerParty.Any(character => character.isAttacking))
        {
            // If the hit object is an enemy
            if (hit.collider.CompareTag("Enemy")) // && isSkillSelected)
            {
                currentTarget = hit.collider.gameObject;
                MoveCursorToTarget();
                outerCircle.SetActive(true);
                activePlayer.attackTarget = currentTarget.transform;
                 Debug.Log("Its getting into the second if statement ok");
                // Execute the queued skill here since an enemy is tapped after selecting a skill
                if ((state == BattleState.PlayerTurn) && isSkillSelected)
                {
                    Debug.Log("Its getting into the final statement ok");
                    skillDescriptionPanel.SetActive(false);
                    ExecuteQueuedSkills();
                   // turnOrderList.RemoveAll(character => character.health <= 0);
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

    public void CheckBattleEnd()
    {
        bool allEnemiesDefeated = enemies.All(enemy => enemy.health <= 0);
        bool allAlliesDefeated = playerParty.All(hero => hero.health <= 0);

        if (allEnemiesDefeated || allAlliesDefeated)
        {
            if (allEnemiesDefeated)
            {
                ProcessVictory();
                GameManager.Instance.currentStageIndex++;
            }
            else if (allAlliesDefeated)
            {
                ProcessDefeat();
            }
        }
    }

    private void DeductStaminaFromParticipants()
    {
        foreach (Character character in playerParty)
        {
            Companion companion = (Companion)character;
            if (companion.stamina > 0)
            {
                companion.stamina -= 1;
                companion.health = companion.maxHealth;
                companion.energy = companion.maxEnergy;
            }
            companion.SaveCharacterData();
        }
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

        PlayerData.Instance.gold += totalGold;
        PlayerData.Instance.SavePlayerData();

        foreach (Character character in playerParty)
        {
            if (character is Companion companion)
            {
                companion.heroExp += totalExp;
                companion.LevelUp();
                companion.SaveCharacterData();
            }
        }

        TextMeshProUGUI expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
        expGainedTextComponent.text = totalExp.ToString();

        TextMeshProUGUI goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
        goldGainedTextComponent.text = totalGold.ToString();

        DisplayExpToLevel(playerParty);
    }

    private void ProcessVictory()
    {
        EndOfBattleRewards(enemies);
        Debug.Log("Battle won");

        Stage completedStage = GameManager.Instance.CurrentBattleConfig.stage;
        GameManager.Instance.UnlockConnectedStages(completedStage);
        if (completedStage.isBossBattle)
        {
            GameManager.Instance.ResetStagesOnBossDefeat();
        }

        endOfBattlePanel.SetActive(true);
        DisplayExpToLevel(playerParty);
    }

    private void ProcessDefeat()
    {
        foreach (Character character in playerParty)
        {
            if (character is Companion companion)
            {
                companion.SaveCharacterData();
            }
        }

        Debug.Log("Battle lost");
        StopAllCoroutines();
        battleLost = true;
        endOfBattleLossPanel.SetActive(true);
    }
}