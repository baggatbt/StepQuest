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

    [Header("Enemy HP Bar Popup")]
[SerializeField] private float hpBarVisibleTime = 0.8f;

private readonly Dictionary<Character, GameObject> _hpUIRootByEnemy = new();
private readonly Dictionary<GameObject, Coroutine> _hideHpCoroutineByUIRoot = new();

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
    
    //public TimingVisualAid visualAid;

    private void Awake()
    {
        // Initialize any necessary components here
         companionSpawnController.CreateHeroSelectionUI();
         //visualAid = FindObjectOfType<TimingVisualAid>(); // Find the visual aid in the scene
    }

private Camera FindBattleCamera()
{
    // 1) Any camera in this scene?
    var thisScene = gameObject.scene;
    foreach (var cam in Camera.allCameras)
        if (cam != null && cam.gameObject.scene == thisScene)
            return cam;

    // 2) Fallback to Camera.main if tagged correctly by PromoteBattleCamera
    return Camera.main;
}


    private void Start()
{
    outerCircleInitialScale = outerCircle.transform.localScale;
    innerCircleInitialScale = innerCircle.transform.localScale;
    // Prefer a camera from THIS scene
    mainCamera = FindBattleCamera();
    Debug.Log($"[Battle] Using camera: {mainCamera?.name} (scene {mainCamera?.gameObject.scene.name})");


    outerCircle.SetActive(false);
    activePlayerIndicator.SetActive(false);
    innerCircle.SetActive(false);

    expGainedTextComponent = ExpGainedText.GetComponent<TextMeshProUGUI>();
    goldGainedTextComponent = GoldGainedText.GetComponent<TextMeshProUGUI>();
    
    

    BattleConfig config = GameManager.Instance.CurrentBattleConfig;
    if (config != null)
    {
        Debug.Log("StartBattle is being called");
    }
    else
    {
        Debug.LogError("No battle configuration found.");
    }

    // Loop through and spawn each enemy
    for (int i = 0; i < config.maxEnemiesToSpawn; i++)
    {
        // Activate the spawn point if it is inactive
        if (!enemySpawnPoints[i].gameObject.activeSelf)
        {
            enemySpawnPoints[i].gameObject.SetActive(true);
        }

        // Spawn each enemy using the modified SpawnEnemiesFromPool method
        Character spawnedEnemy = enemySpawnController.SpawnEnemiesFromPool(
            config.poolName,
            enemySpawnPoints[i],     // Spawn point for this enemy
            healthBars[i],           // Health bar for this enemy
            energyBars[i],           // Energy bar for this enemy
            i,                       // Health text index
            config.levelOfEnemies,    // Enemy level
            enemyHealthUI[i]          // Health UI for this enemy
        );

       if (spawnedEnemy != null)
{
    enemies.Add(spawnedEnemy);

    if (i < enemyHealthUI.Length && enemyHealthUI[i] != null)
{
    _hpUIRootByEnemy[spawnedEnemy] = enemyHealthUI[i];
    enemyHealthUI[i].SetActive(true);
   // SnapEnemyHpUIToEnemy(spawnedEnemy, ignoreFreeze: true);
}
}

        
    }

    // Uncomment to start the battle after enemies are spawned
    // StartBattle(config);
}

[SerializeField] private bool freezeEnemyHpUIWhileZooming = true;
private bool _freezeEnemyHpUI;
// Cache last known-good slider local position per enemy
private readonly Dictionary<Character, Vector3> _lastHpSliderLocalPos = new();
// One-shot reposition jobs so multiple hits don't stack coroutines
private IEnumerator RepositionEnemyHpBarAfterUIRebuild(Character enemy)
{
    // Wait 1 frame so enabling the GameObject doesn't get overridden by layout rebuilds
    yield return null;

    // If there are LayoutGroups / ContentSizeFitters, they often rebuild at end of frame too
    yield return new WaitForEndOfFrame();

    // Force UI rebuild to be extra safe
    Canvas.ForceUpdateCanvases();

    // ✅ Snap ONCE even if we're currently zooming (bypass freeze)
   // SnapEnemyHpUIToEnemy(enemy, ignoreFreeze: true);

    // cleanup
    _repositionHpCoroutineByEnemy.Remove(enemy);
}
private readonly Dictionary<Character, Coroutine> _repositionHpCoroutineByEnemy = new();
public void PopupEnemyHealthBar(Character enemy)
{
    if (enemy == null) return;
    if (enemy.healthBar == null) return;

    if (!_hpUIRootByEnemy.TryGetValue(enemy, out var uiRoot) || uiRoot == null)
        return;

    uiRoot.SetActive(true);
}

private IEnumerator HideHpAfterDelay(GameObject uiRoot, float delay)
{
    yield break;
}

public void ResetAndSetupForStage(BattleConfig config)
{
    if (config == null)
    {
        Debug.LogError("[Battle] Reset called with null config.");
        return;
    }

    Debug.Log($"[Battle] Resetting for new stage config: {config.poolName}");

    // Stop battle coroutines
    StopAllCoroutines();

    // IMPORTANT: reset all party members runtime flags (they persist across stages)
    foreach (var p in playerParty)
        ResetCharacterRuntime(p);

    // reset list/state
    battleLost = false;
    isBattleStarted = false;
    state = BattleState.PlayerTurn;
    currentTarget = null;
    isSkillSelected = false;

    skillQueue.Clear();
    requestedSkill = null;

    turnOrderList.Clear();

    // Destroy old enemies + reset their runtime first
    for (int i = enemies.Count - 1; i >= 0; i--)
    {
        var e = enemies[i];
        if (e != null) ResetCharacterRuntime(e);
        if (e != null) Destroy(e.gameObject);
    }
    enemies.Clear();

    // UI reset
    if (outerCircle) outerCircle.SetActive(false);
    if (innerCircle) innerCircle.SetActive(false);
    if (activePlayerIndicator) activePlayerIndicator.SetActive(false);

    if (endOfBattlePanel) endOfBattlePanel.SetActive(false);
    if (endOfBattleLossPanel) endOfBattleLossPanel.SetActive(false);
    if (heroSelectionPanel) heroSelectionPanel.SetActive(true);
    if (battleStartButton) battleStartButton.SetActive(true);

    // Clear turn order bar icons
    if (turnOrderBarPanel)
        foreach (Transform child in turnOrderBarPanel.transform)
            Destroy(child.gameObject);

    // Re-acquire camera AFTER scene switch (don’t overwrite with Camera.main elsewhere)
    mainCamera = FindBattleCamera();

    // Respawn enemies for the new config
    for (int i = 0; i < config.maxEnemiesToSpawn; i++)
    {
        if (i >= enemySpawnPoints.Length || i >= healthBars.Length || i >= energyBars.Length || i >= enemyHealthUI.Length)
        {
            Debug.LogWarning("[Battle] Not enough UI/spawn slots for maxEnemiesToSpawn.");
            break;
        }

        enemySpawnPoints[i].gameObject.SetActive(true);

        Character spawnedEnemy = enemySpawnController.SpawnEnemiesFromPool(
            config.poolName,
            enemySpawnPoints[i],
            healthBars[i],
            energyBars[i],
            i,
            config.levelOfEnemies,
            enemyHealthUI[i]
        );

        if (spawnedEnemy != null)
        {
            ResetCharacterRuntime(spawnedEnemy);   // <-- KEY: ensure clean flags
            enemies.Add(spawnedEnemy);
        }
    }

    Debug.Log("[Battle] Reset complete.");
}

private void SnapEnemyHpUIToEnemy(Character enemy, bool ignoreFreeze = false)
{
    if (enemy == null) return;
    if (enemy.healthBar == null) return;

    if (mainCamera == null) mainCamera = FindBattleCamera();
    if (mainCamera == null) return;

    // Freeze only blocks continuous snapping — but popup needs a one-shot snap
    if (!ignoreFreeze && freezeEnemyHpUIWhileZooming && _freezeEnemyHpUI)
        return;

    var slider = enemy.healthBar;
    var parentRT = slider.transform.parent as RectTransform;
    if (parentRT == null) return;

    // World position above enemy
    Renderer r = enemy.GetComponentInChildren<Renderer>();
    float topY = (r != null) ? r.bounds.max.y : enemy.transform.position.y;
    Vector3 worldPos = new Vector3(enemy.transform.position.x, topY + 0.5f, enemy.transform.position.z);

    // World -> screen (battle camera)
    Vector3 screen = mainCamera.WorldToScreenPoint(worldPos);

    // Screen -> local UI
    // (If your Canvas is Overlay, uiCam should be null. If ScreenSpaceCamera, use its worldCamera.)
    Canvas canvas = slider.GetComponentInParent<Canvas>();
    Camera uiCam = null;
    if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        uiCam = canvas.worldCamera != null ? canvas.worldCamera : mainCamera;

    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        parentRT,
        screen,
        uiCam,
        out localPoint
    );

    // ✅ Your "spawner style": move the slider itself
    slider.transform.localPosition = localPoint;

    // Cache for debugging/optional restores
    _lastHpSliderLocalPos[enemy] = slider.transform.localPosition;
}

private void ResetCharacterRuntime(Character c)
{
    if (c == null) return;

    // Character is a MonoBehaviour in your project, so this is valid
    c.StopAllCoroutines();

    c.isAttacking = false;
    c.isMoving = false;

    c.hasNotGone = true;          // your turn gating flag
    c.currentSkill = null;
    c.attackTarget = null;

    // Optional safety if you use Animator booleans/triggers
    var anim = c.GetComponent<Animator>();
    if (anim != null)
    {
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Hit");
        anim.ResetTrigger("Die");
        anim.Play(0, 0, 0f);      // restart base state
    }
}




    public GameObject knightPrefab;
    

    
   public void SetupBattle(List<Companion> currentParty)
    {
        int companionIndex = 0; // Initialize the index
        foreach (var companion in currentParty)
        {
            
                playerParty.Add(companion);
            
        }
    }
    

    public void StartBattle()
{
    const int battleCostPerHero = 0;          // cost to join this battle

    // NEW stamina gate 
    foreach (var c in playerParty.OfType<Companion>())
        if (!c.TrySpendStamina(battleCostPerHero))
        {
            Debug.Log($"{c.heroID} is exhausted ─ visit Recovery Center.");
            return;                           // abort start
        }
    

    if (playerParty.Count > 0)
    {
        if (isBattleStarted) return;          // prevent double-start
        battleStartButton.SetActive(false);
        isBattleStarted = true;

        endOfBattlePanel.SetActive(false);
        heroSelectionPanel.SetActive(false);

        if (playerParty.Count > 0) companion1 = playerParty[0];
        if (playerParty.Count > 1) companion2 = playerParty[1];

        activePlayer = companion1;            // default
        InitializeTurnOrder();
    }
}


    public void RestoreHealthAndEnergy()
    {
        foreach (var companion in playerParty.OfType<Companion>())
        {
           // companion.energy = companion.maxEnergy;
           // companion.health = companion.maxHealth;
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
    private int GetSortPriority(Character c)
{
    // Higher = earlier
    return (c != null && c.currentSkill != null) ? c.currentSkill.priority : 0;
}

private int GetEffectiveSpeedForTurnOrder(Character c)
{
    if (c == null) return 0;

    // If a skill is selected for this turn, it modifies speed
    if (c.currentSkill != null)
        return c.currentSkill.GetEffectiveSpeed(c.speed);

    // Otherwise use raw speed
    return c.speed;
}
[SerializeField] private bool resolvingPhase = false;
private void BeginPlanningPhase()
{
    if (battleLost) return;
    planningPhase = true;
    resolvingPhase = false;
    SetAllEnemyHpBarsVisible(true);


    plannedActions.Clear();
    skillQueue.Clear();
    requestedSkill = null;
    currentTarget = null;
    isSkillSelected = false;

    // Reset so players can plan again
    foreach (var p in playerParty)
        if (p != null && p.health > 0)
            p.hasNotGone = true;

    // Enemies pre-plan now (skill + target)
    foreach (var e in enemies)
    {
        if (e == null || e.health <= 0) continue;

        e.currentSkill = e.normalSkill != null ? e.normalSkill : e.currentSkill;

        var t = SelectTargetForEnemy();
        e.attackTarget = t;

        plannedActions[e] = new PlannedAction
        {
            actor = e,
            skill = e.currentSkill,
            target = t
        };
    }

    // Set activePlayer to first living companion that still needs to plan
    activePlayer = GetNextPlanningPlayer();

    // Only show player UI if there IS someone to plan
    if (activePlayer != null)
    {
        ChangeState(BattleState.PlayerTurn);
        StartCoroutine(EnableAllButtons());
    }
    else
    {
        // No players alive to plan -> go straight to resolution (enemies only)
        FinalizePlansAndBuildTurnOrder();
    }

    Debug.Log("[OptionB] Planning phase started.");
}

private Character GetNextPlanningPlayer()
{
    // pick next companion in party that is alive and hasNotGone == true
    foreach (var p in playerParty)
    {
        if (p == null) continue;
        if (p.health <= 0) continue;
        if (p.hasNotGone) return p;
    }
    return null;
}



private void FinalizePlansAndBuildTurnOrder()
{
    planningPhase = false;
    resolvingPhase = true;

    SetAllEnemyHpBarsVisible(false);

    turnOrderList.Clear();

    foreach (var kv in plannedActions)
    {
        var actor = kv.Key;
        var plan = kv.Value;

        if (actor == null || actor.health <= 0) continue;
        if (plan == null || plan.skill == null) continue;

        actor.currentSkill = plan.skill;
        actor.attackTarget = plan.target;

        turnOrderList.Add(actor);
    }

    // Clear existing icons
    if (turnOrderBarPanel != null)
        foreach (Transform child in turnOrderBarPanel.transform)
            Destroy(child.gameObject);

    // Sort by priority then effective speed
    turnOrderList = turnOrderList
        .OrderByDescending(c => (c != null && c.currentSkill != null) ? c.currentSkill.priority : 0)
        .ThenByDescending(c => GetEffectiveSpeedForTurnOrder(c))
        .ToList();

    // Rebuild icons
    foreach (var character in turnOrderList)
    {
        if (character == null) continue;

        GameObject icon = Instantiate(iconPrefab, turnOrderBarPanel.transform);
        Image iconImage = icon.GetComponent<Image>();

        if (character is Companion)
            iconImage.sprite = (character as Companion).heroIcon;
        else if (character is Enemy)
            iconImage.sprite = (character as Enemy).enemyIcon;
    }

    DisableAllButtons(); // resolution should not allow choosing new moves

    Debug.Log("[OptionB] Plans locked. Turn order built.");
    StartTurn();
}

private IEnumerator ResolvePlannedPlayerTurn(Character playerActor)
{
    // Wait until nobody is mid-attack/move
    yield return new WaitUntil(() =>
        !playerParty.Any(p => p != null && p.isAttacking) &&
        !enemies.Any(e => e != null && e.isAttacking)
    );

    activePlayer = playerActor;

    // Planned target from plan
    var t = playerActor.attackTarget;
    currentTarget = (t != null) ? t.gameObject : null;

    if (playerActor.currentSkill == null || currentTarget == null)
    {
        Debug.LogWarning($"[OptionB] Missing planned skill/target for {playerActor?.name}. Skipping turn.");
        EndTurn();
        yield break;
    }

    // IMPORTANT: do NOT prompt the player again during resolution
    DisableAllButtons();

    // Run the same execution pipeline you already use
    yield return StartCoroutine(PlayerAction());

    // PlayerAction/Attack coroutines usually end without calling EndTurn in OptionB
    // so we end the turn here.
    EndTurn();
}

    // --------------------
// Option B: Planning phase
// --------------------
[Header("Option B Planning")]
[SerializeField] private bool planningPhase = false;

// Planned action per character for the upcoming resolution
private readonly Dictionary<Character, PlannedAction> plannedActions = new Dictionary<Character, PlannedAction>();

[Serializable]
private class PlannedAction
{
    public Character actor;
    public Skill skill;
    public Transform target;   // can be null until chosen
}
    public void InitializeTurnOrder()
{
    // Option B: this should only happen after planning
    // If someone calls it by accident, push into planning instead of sorting raw speed.
    Debug.LogWarning("[OptionB] InitializeTurnOrder called. Redirecting to BeginPlanningPhase().");
    BeginPlanningPhase();
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
        BeginPlanningPhase();
    }
    }
}

    private void ExecuteTurn(Character character)
{
    if (character == null || character.health <= 0)
    {
        EndTurn();
        return;
    }

    // During resolution: players auto-execute their planned action
    if (resolvingPhase && (character == companion1 || character == companion2))
    {
        // Set battle state without re-enabling UI
        state = BattleState.PlayerTurn; // intentionally NOT ChangeState()
        StartCoroutine(ResolvePlannedPlayerTurn(character));
        return;
    }

    // Normal enemy turn resolution
    if (character != companion1 && character != companion2)
    {
        ChangeState(BattleState.EnemyTurn);
        EnemyAttack(character);
        return;
    }

    // Planning phase: allow choosing a move
    activePlayer = character;
    ChangeState(BattleState.PlayerTurn);
}

    public void EndTurn()
{
    if (turnOrderList.Count > 0)
        turnOrderList.RemoveAt(0);

    turnOrderList.RemoveAll(character => character == null || character.health <= 0);

    if (turnOrderList.Count == 0)
    {
        // End of resolution round
        if (statusEffectController != null)
            statusEffectController.ProcessEffects();

        BeginPlanningPhase(); // start next round planning
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
       // enemyUIPanel.SetActive(false);
      //  heroUIPanels.SetActive(false);
    }

    public GameObject enemyUIPanel;
    public GameObject heroUIPanels;
    public GameObject companionSkillsPanel;
    public GameObject SkillsPanel;

    private bool ShouldShowCombatUI()
{
    // Only show UI when the player is actively PLANNING a move.
    return isBattleStarted
        && !battleLost
        && planningPhase
        && !resolvingPhase
        && state == BattleState.PlayerTurn
        && activePlayer != null
        && activePlayer.hasNotGone;   // still needs to plan
}

private void SetAllEnemyHpBarsVisible(bool visible)
{
    foreach (var e in enemies)
    {
        if (e == null) continue;
        if (e.health <= 0) continue;

        if (_hpUIRootByEnemy.TryGetValue(e, out var uiRoot) && uiRoot != null)
            uiRoot.SetActive(true);
        else if (e.enemyHealthUI != null)
            e.enemyHealthUI.SetActive(true);
        else if (e.healthBar != null)
            e.healthBar.gameObject.SetActive(true);
    }

    foreach (var kv in _hideHpCoroutineByUIRoot)
        if (kv.Value != null) StopCoroutine(kv.Value);

    _hideHpCoroutineByUIRoot.Clear();
}
    public IEnumerator EnableAllButtons()
{
    yield return new WaitForSeconds(0.25f);

    if (!ShouldShowCombatUI())
    {
        // Keep everything hidden during resolution
        companionSkillsPanel.SetActive(false);
        SkillsPanel.SetActive(false);
       // enemyUIPanel.SetActive(false);
        heroUIPanels.SetActive(false);
        yield break;
    }

    // Planning UI
    companionSkillsPanel.SetActive(true);
    SkillsPanel.SetActive(true);
   // enemyUIPanel.SetActive(true);
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
      //  radialMenuPanelCenterPoint.transform.position = new Vector3(activePlayer.transform.position.x, activePlayer.transform.position.y + 3f, activePlayer.transform.position.z);
    }

    public void ExecuteQueuedSkills()
{
    if (currentTarget == null || state != BattleState.PlayerTurn)
    {
        Debug.Log("No target selected / not player turn.");
        return;
    }

    // OPTION B: If we're in planning phase, committing a plan replaces immediate execution
    if (planningPhase)
    {
        if (skillQueue.Count == 0)
        {
            Debug.Log("[OptionB] No skill queued to commit.");
            return;
        }

        Skill skill = skillQueue.Dequeue();
        activePlayer.currentSkill = skill;
        activePlayer.attackTarget = currentTarget.transform;

        // Spend energy NOW to lock the choice (matches your current behavior)
        activePlayer.SpendEnergy(skill.energyCost);

        plannedActions[activePlayer] = new PlannedAction
        {
            actor = activePlayer,
            skill = skill,
            target = currentTarget.transform
        };

        activePlayer.hasNotGone = false; // finished planning
        isSkillSelected = false;
        skillDescriptionPanel.SetActive(false);

        Debug.Log($"[OptionB] Planned: {activePlayer.name} uses {skill.skillName} on {currentTarget.name}");

        // Move to next planner
        activePlayer = GetNextPlanningPlayer();

        if (activePlayer != null)
        {
            // Still planning another companion
            StartCoroutine(EnableAllButtons());
            return;
        }

        // Everyone planned -> lock plans and compute order
        DisableAllButtons();
        FinalizePlansAndBuildTurnOrder();
        return;
    }

    // ORIGINAL behavior (if you ever turn planningPhase off manually)
    Debug.Log("Executing queued skills on target: " + currentTarget.transform.position);
    DisableAllButtons();
    StartCoroutine(ExecuteAllSkillsCoroutine());
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
        if (freezeEnemyHpUIWhileZooming) _freezeEnemyHpUI = true;   // ✅ FREEZE
        StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position));
    }

    // Only move if the player is not already at the target
    if (activePlayer.transform.position != currentTarget.transform.position)
    {
        activePlayer.originalPosition = activePlayer.transform.position;
        Debug.Log("OP set to " + activePlayer.originalPosition);
        yield return activePlayer.MoveToTarget();
    }

    // Attack
    yield return StartCoroutine(PlayerAttackCoroutine(null));

    // Note: zoom-out + unfreeze happens in PlayerAttackCoroutine in your code,
    // so we do NOT duplicate it here.
}

    public IEnumerator PlayerAttackCoroutine(System.Action successCallback)
    {
        
        Character targetEnemy = currentTarget.GetComponent<Character>();
        yield return new WaitUntil(() => targetEnemy.isAttacking == false);

        if (activePlayer.currentSkill != null)
        {
            
            yield return activePlayer.currentSkill.Execute(activePlayer, targetEnemy, this);
        // Award Knight mastery/progression (Section C)
        if (activePlayer is Companion c && c.heroID == "Knight")
        {
            var usedSkill = activePlayer.currentSkill;
            // Safety: if Type wasn't set for some reason, do nothing
            if (usedSkill != null && usedSkill.Type != SkillType.None)
            {
                // LastTimingResult was set inside the Skill timing handlers
                ProgressionEvents.OnKnightSkillUsed(activePlayer, usedSkill.Type, usedSkill.LastTimingResult);
            }
        }
           // CheckBattleEnd();
        }
        if (activePlayer.currentSkill.requiresMovement == true)
        {
        // Move player back to their original position after all skills executed.

        if (!activePlayer.currentSkill.noZoom)
{
    StartCoroutine(zoomEffect.ZoomOutEffect());
    if (freezeEnemyHpUIWhileZooming) _freezeEnemyHpUI = false; // ✅ UNFREEZE
}
        
        if (ShouldShowCombatUI())
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
    // Option B: activePlayer can be null (enemy may go first)
    yield return new WaitUntil(() =>
        !playerParty.Any(p => p != null && p.isAttacking) &&
        !enemies.Any(e => e != null && e.isAttacking)
    );

    yield return new WaitForSeconds(1.0f);

    Transform enemyTargetTransform = currentEnemy.attackTarget;

    // If target died or was null, pick a new one (fallback safety)
    if (enemyTargetTransform == null ||
        enemyTargetTransform.GetComponent<Character>() == null ||
        enemyTargetTransform.GetComponent<Character>().health <= 0)
    {
        enemyTargetTransform = SelectTargetForEnemy();
        currentEnemy.attackTarget = enemyTargetTransform;
    }

    currentTarget = enemyTargetTransform != null ? enemyTargetTransform.gameObject : null;
    if (currentTarget == null)
    {
        Debug.LogWarning("[OptionB] Enemy had no valid target.");
        EndTurn();
        yield break;
    }

    if (currentEnemy.currentSkill != null)
    {
        if (currentEnemy.currentSkill.requiresMovement)
        {
            if (freezeEnemyHpUIWhileZooming) _freezeEnemyHpUI = true; // ✅ FREEZE
            StartCoroutine(zoomEffect.ZoomCameraEffect(currentTarget.transform.position));

            DisableAllButtons();
            yield return currentEnemy.MoveToTarget();

            // After MoveToTarget() finishes and *before* playing the attack animation:
            var motion = currentEnemy.GetComponent<AttackMotionController>();
            if (motion != null && currentTarget != null)
            {
                motion.currentTarget = currentTarget.transform;
                motion.SetContactAnchorFromCurrent();
            }
        }

        Character targetCharacter = enemyTargetTransform.GetComponent<Character>();
        yield return currentEnemy.currentSkill.Execute(currentEnemy, targetCharacter, this);

        if (currentEnemy.currentSkill.requiresMovement)
        {
            StartCoroutine(zoomEffect.ZoomOutEffect());

            if (freezeEnemyHpUIWhileZooming) _freezeEnemyHpUI = false; // ✅ UNFREEZE

            yield return currentEnemy.ReturnToPosition();

            if (ShouldShowCombatUI())
                StartCoroutine(EnableAllButtons());
        }
    }
    else
    {
        Debug.Log("Standard Attack Performed - This should not happen");
    }

    EndTurn();
}
    [Header("Popups")]
[SerializeField] private RectTransform popupParent;   // assign in inspector
[SerializeField] private Canvas popupCanvas;          // assign in inspector (same canvas as parent)

    public CameraShake cameraShake;

    public void ShowTimingResult(string message)
{
    Debug.Log($"[Popup] BattleManager instance: {name} id={GetInstanceID()} scene={gameObject.scene.name}");
Debug.Log($"[Popup] popupParent: {popupParent.name} id={popupParent.GetInstanceID()} scene={popupParent.gameObject.scene.name}");
Debug.Log($"[Popup] popupCanvas: {popupCanvas.name} id={popupCanvas.GetInstanceID()} scene={popupCanvas.gameObject.scene.name}");

    if (currentTarget == null) return;

    if (popupParent == null || popupCanvas == null)
    {
        Debug.LogError("[Battle] popupParent/popupCanvas not assigned on BattleManager.");
        return;
    }

    var prefab = Resources.Load<GameObject>("Prefab/UI Elements/TimingResultPopup");
    if (prefab == null)
    {
        Debug.LogError("[Battle] Failed to load TimingResultPopup prefab.");
        return;
    }

    // Convert enemy world pos -> UI local pos
    Vector3 world = currentTarget.transform.position + Vector3.up * 3f;
    Vector2 screen = RectTransformUtility.WorldToScreenPoint(mainCamera, world);

    Camera uiCam = (popupCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : popupCanvas.worldCamera;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        popupParent, screen, uiCam, out Vector2 localPoint
    );

    var go = Instantiate(prefab, popupParent);
    Debug.Log($"[Popup] spawned parent: {go.transform.parent.name} scene={go.scene.name}");

    var rt = go.GetComponent<RectTransform>();
    rt.anchoredPosition = localPoint;
    go.transform.SetAsLastSibling();

    var dmg = go.GetComponent<DamagePopup>();
    if (dmg != null) dmg.SetupTimingEventResult(message);
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

    //Works with new timing system to control shake effect on hit
    public void CameraShakeMagnitude(TimingEventResult result)
    {
        if (result == TimingEventResult.Good)
            {
                StartCoroutine(cameraShake.Shake(0.5f));
               // return TimingEventResult.Good;
            }
                else if (result != TimingEventResult.Good)
            {
                StartCoroutine(cameraShake.Shake(0.25f));
               // return TimingEventResult.Early;
            }
                else
            {
                StartCoroutine(cameraShake.Shake(0.25f));
                //return TimingEventResult.Late;
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
    if (!isBattleStarted) return;

    MoveRadialMenuToActivePlayer();

    // ✅ Only allow taps during planning, not during resolution
    if (!planningPhase) return;
    if (state != BattleState.PlayerTurn) return;
    if (activePlayer == null) return;
    if (!activePlayer.hasNotGone) return;
    if (!playerParty.Any(character => character.isAttacking) && !enemies.Any(character => character.isAttacking))
    {
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider == null)
    Debug.Log("Raycast hit nothing.");

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
               // companion.health = companion.maxHealth;
                //companion.energy = companion.maxEnergy;
            }
            companion.SaveCharacterData();
        }
    }

    public void EndOfBattleRewards(List<Character> enemies)
    {
        int totalExp = 0;
        int totalCopper = 0;

        foreach (Enemy enemy in enemies)
        {
            totalExp += enemy.expReward;
            totalCopper += enemy.goldReward;
        }

        PlayerData.Instance.totalCopper += totalCopper;
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
        goldGainedTextComponent.text = totalCopper.ToString();

        DisplayExpToLevel(playerParty);
    }

    private void ProcessVictory()
{
    EndOfBattleRewards(enemies);
    KnightProgressionService.TryApply(GameManager.Instance.currentCompanionData);

    var sd = GameManager.Instance.currentStage;   // StageData of the battle you just won
    if (sd != null)
    {
        GameManager.Instance.UnlockConnectedStagesForRun(sd);  // <-- unlock neighbors for this RUN
    }
    else
    {
        Debug.LogError("[ProcessVictory] GameManager.currentStage is null; cannot unlock neighbors.");
    }

    endOfBattlePanel.SetActive(true);
    FindObjectOfType<RunLootPanelUI>(true).Show();

    DisplayExpToLevel(playerParty);
}



    public void GoToNextStage()
    {
        
          
         GameManager.Instance.GetNextStage(0); // Transitions to the first connected stage
    }

    private void ProcessDefeat()
{
    foreach (Character character in playerParty)
        if (character is Companion companion)
            companion.SaveCharacterData();

    Debug.Log("Battle lost");
    StopAllCoroutines();
    battleLost = true;

    // End run on defeat
    GameManager.Instance.EndDungeonRun(false);

    
    GameManager.Instance.ReturnToTownFromBattle();
    endOfBattleLossPanel.SetActive(true);
}

// BattleManager.cs
public void ReturnToMapAfterVictory()
{
    // neighbors already unlocked in ProcessVictory()
    //Scoop up rewards
    GameManager.Instance.EndDungeonRun(true);

    StartCoroutine(ExitBattleRoutine());
}

private IEnumerator ExitBattleRoutine()
{
    var battleSceneName = SceneManager.GetActiveScene().name;
    GameManager.Instance.ReturnToTownFromBattle();
    // re-enable town raycasts BEFORE unloading battle
    GameManager.Instance.SetUIRaycastsForScene("CharacterInfoPage", true);

    yield return SceneManager.UnloadSceneAsync(battleSceneName);
}


}