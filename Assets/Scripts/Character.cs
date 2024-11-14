 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System;



public class Character : MonoBehaviour 
{
    public int level;
    public int health;
    public int maxHealth;
    public string characterIDNumber;
    public bool isSelected;
    
    public int energy;
    public int maxEnergy;
    public int teamEnergy;
    public int attackPower;
    public int defensePower;
    public int defensePenetration;
    public float enemyDamageReductionModifier;
    public int speed;
    public Slider healthBar;
    public Slider energyBar;
    public Transform attackTarget;
    public Vector3 originalPosition;
    public bool isAttacking = false;
    public bool isFront;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public TextMeshProUGUI healthText; 
    public TextMeshProUGUI energyText;
   // public GameObject enemyHealthUI;
    
     

    public bool hasNotGone;
    public bool isMoving;
    public bool attackTrigger;
    public bool animationEnded;
    public bool didBlock;
    public Skill currentSkill; 
    public List<Skill> skills = new List<Skill>();
    public int attacksBeforeSpecial; 
    public Skill normalSkill; 
    public Skill specialSkill;
    
    public GameObject enemyHealthUI;

    //Testing various statuses
    public float damageReflectionPercentage; // Percentage of damage to reflect

    
   
    private bool checkCollisionsDuringMovement = true;

    //STATUS EFFECTS
    public StatusEffectController statusEffectController;


    
    

    protected virtual void Awake()
    {
        isSelected = false;
        
        animator = GetComponent<Animator>();
        statusEffectController = GetComponent<StatusEffectController>();
        enemyDeathEffect = GetComponent<EnemyDeathEffect>();
        // Ensure originalPosition is set to the character's initial position if not already set
        if (originalPosition == Vector3.zero || originalPosition == new Vector3(10000f, 10000f, -16.20f))
        {
            originalPosition = transform.position;
            Debug.Log($"OP at Awake() {originalPosition}");
        }
     //   originalPosition = transform.position;
        Debug.Log("OP at Awake() " + originalPosition);
       // this.hasNotGone = true;
    }

    

    
    public void Update()
    {
            
        if (this.healthBar != null)
        {
            
            this.healthBar.maxValue = maxHealth;
            this.healthBar.value = health;
            if (this.healthText != null )
            {
                
                this.healthText.text =  this.health + "";
                
            }
        }
        if (this.energyBar != null)
        {
            energyBar.maxValue = maxEnergy;
            energyBar.value = this.energy;
            if (this.energyText != null)
            {
                
                this.energyText.text = this.energy + "";
                this.energyBar.value = this.energy;
                
                
            }
        }
        

    }
    

    protected virtual void Start()
    {
        
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
            if (healthText !=null && energyText !=null)
            {
               this.healthText.text = this.health +"";
                energyText.text = this.energy +"";
            }
        }
        

        if (energyBar != null)
        {
            energyBar.maxValue = maxEnergy;
            energyBar.value = energy;
        }

        
        
    }

    

    public CameraShake cameraShake; // Reference to the CameraShake script

/*
    public void EnemyTakeDamage(int damageOfAttacker, Character attacker, float enemyDamageReductionModifier)
    {
        float damageReduction = (int)((damageOfAttacker * enemyDamageReductionModifier));
        int damageDealt = (int)((damageOfAttacker - damageReduction));
        Debug.Log("Dealt " + damageDealt + " damage!");

        Debug.Log("Damage reduced by Defense: " + (damageOfAttacker - damageDealt));

    health -= damageDealt;
    healthBar.value = health;

    // Update the health text.
    if (this.healthText != null)
    {
        this.healthText.text =  this.health + " / " + this.maxHealth;
    }
    
    if (damageDealt > 0)
    { 
       // IsHit();
       // Damage popup
    GameObject damagePopupPrefab = Resources.Load<GameObject>("PreFab/DamagePopup");

    // Find the EndOfBattleRewards GameObject in the scene
    Transform endOfBattleRewardsTransform = GameObject.Find("EndOfBattleRewardsCanvas").transform;

    if(damagePopupPrefab != null)
    {
        // Instantiate the damage popup as a child of the EndOfBattleRewards GameObject
        GameObject damagePopupInstance = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity, endOfBattleRewardsTransform);

        DamagePopup damagePopupScript = damagePopupInstance.GetComponent<DamagePopup>();
        damagePopupScript.Setup(damageDealt);
        
        
    }
    else
    {
        Debug.LogError("Failed to load DamagePopup prefab.");
    }
    }
    Debug.Log("isAttacking = " +attacker.isAttacking);
    }

*/


    

    public void TakeDamage(int damageOfAttacker, Character attacker)
{
    if (currentBarrier != null && currentBarrier.IsActive)
        {
            currentBarrier.AbsorbDamage();
            
            return; // Skip taking damage because the barrier absorbed it
        }
        
    // Calculate effective defense after penetration
      int effectiveDefense = Math.Max(0, defensePower - attacker.defensePenetration);
   // Debug.Log("The target has " + effectiveDefense + " defense left after penetration.");

    // Calculate total damage after flat defense reduction
    int totalDamageAfterDefense = damageOfAttacker - effectiveDefense;
    

    Debug.Log("Total damage dealt  = " + totalDamageAfterDefense);

    // Subtract the calculated damage from health
    this.health -= totalDamageAfterDefense;
    if (this.healthBar != null)
    {
    this.healthBar.value = health;
    }
    this.EnemyIsHit(); //If the character taking damage is an enemy, they all share the same trigger
    Debug.Log("Takign damage but im null" + this.healthText);
    // Update the health text
    if (this.healthText != null)
    {
        this.healthText.text = this.health + "";
    }
    
    // Check if damage was dealt for additional effects
    if (totalDamageAfterDefense >= 1)
    { 
        Debug.Log("did block from character" + this.didBlock);
        if (!this.didBlock){
        this.animator.SetTrigger("IsHurtTrigger");
        StartCoroutine(TintRed());
        }
        // Trigger hit reaction, damage popup, etc.
        int damageToReflect = (int)Math.Ceiling(damageReflectionPercentage * totalDamageAfterDefense);
        attacker.health -= damageToReflect;
        GameObject damagePopupPrefab = Resources.Load<GameObject>("PreFab/DamagePopup");
        Transform endOfBattleRewardsTransform = GameObject.Find("EndOfBattleRewardsCanvas").transform;
        if(damagePopupPrefab != null)
        {
            GameObject damagePopupInstance = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity, endOfBattleRewardsTransform);
            DamagePopup damagePopupScript = damagePopupInstance.GetComponent<DamagePopup>();
            damagePopupScript.Setup(totalDamageAfterDefense);
        }
        else
        {
            Debug.LogError("Failed to load DamagePopup prefab.");
        }
    }
    Debug.Log("isAttacking = " + attacker.isAttacking);
    
   
}
public float tintDuration = 0.1f; // Duration of the tint
private IEnumerator TintRed()
    {
        // Store the original color of the sprite
        Color originalColor = spriteRenderer.color;

        // Set the sprite color to red
        spriteRenderer.color = Color.red;

        // Wait for the tint duration
        yield return new WaitForSeconds(tintDuration);

        // Revert the sprite color to its original color
        spriteRenderer.color = originalColor;
    }


    IEnumerator FadeOutSprite()
{
    SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
    if (sr == null) yield break;  // If no SpriteRenderer, exit the coroutine

    float fadeDuration = 0.25f; // duration for the fade, 
    float currentTime = 0.0f;

    Color originalColor = sr.color;

    while (currentTime < fadeDuration)
    {
        currentTime += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0, currentTime / fadeDuration);
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        yield return null;
    }

    sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    this.gameObject.SetActive(false);  // deactivate the GameObject after fade

    if (healthBar != null)
    {
    healthBar.gameObject.SetActive(false);  // Disable the healthBar's GameObject
    }

    if (energyBar != null)
    {
        energyBar.gameObject.SetActive(false);  // Disable the energyBar's GameObject
    }
    
        enemyHealthUI.SetActive(false); //Will also work for heroes
    

     yield return new WaitForSeconds(0.5f); //Ensures fade is all done
    }

   // private int remainingCost = 0;
    public void SpendEnergy(int energySpent)
{
    this.energy -= energySpent;
    
    // Check if energyBar and energyText are not null before accessing their properties
    if (energyBar != null)
    {
        energyBar.value = this.energy;
    }

    if (energyText != null)
    {
        energyText.text = this.energy.ToString();
    }
}


/*
    public void GainTeamEnergy(int energyGained)
{
    if (PlayerData.Instance.teamEnergy + energyGained > maxEnergy) // If the gained energy will bring the total over the max
    {
        PlayerData.Instance.teamEnergy = maxEnergy; // Set energy to the max
        Debug.Log("Passing first if in gainteamenergy");
        

    }
    if (PlayerData.Instance.teamEnergy + energyGained < maxEnergy) // If the gained energy will not bring the total over the max
    {
        PlayerData.Instance.teamEnergy += energyGained; // Add the gained energy to the total
        Debug.Log("Passing second if in gainteamenergy");
       
    }
    
    if (energyBar != null)
    {
        
        energyBar.value = PlayerData.Instance.teamEnergy;
        
    }
    if (energyText != null) //Had to separate this because monster energies do not use text.
    {
        energyText.text =  this.energy + "";
    }
    Debug.Log("Not passing any");
}
*/
    //For enemy rage bars
   public void GainEnergy(int energyGained)
{
    if (this.energy < this.maxEnergy)
    {
        Debug.Log("Gaining " + energyGained + " energy");
        this.energy += energyGained; // Corrected to add energyGained to the current energy
        if (this.energy > this.maxEnergy) // Ensuring that energy does not exceed maxEnergy
        {
            this.energy = this.maxEnergy;
        }
        Debug.Log(this.energy.ToString() + " total energy now");
    }
    if (energyBar != null)
    {
        energyBar.value = this.energy;
    }

    if (energyText != null)
    {
        energyText.text = this.energy.ToString();
    }
}


    public void Gainhealth(int healthGained)
    {
        this.health = healthGained;
    }

   
    public void EnemyIsHit() => animator.SetTrigger("EnemyIsHurtTrigger");
    
    public bool isAnimationDone = false;

    //Called at points in an animation to flag that its over and the animation can continue or stop
    public void AnimationEnded()
    {
         Debug.Log("animation ended!");
         isAnimationDone = true;
        
    }

    public Barrier currentBarrier;
    public void AddBarrier(GameObject barrierPrefab)
    {
        if (currentBarrier == null) // Ensure there isn't already a barrier
        {
            GameObject barrierObj = Instantiate(barrierPrefab, transform.position, Quaternion.identity, transform);
            currentBarrier = barrierObj.GetComponent<Barrier>();
            
        }
    }

   

    //This will be called on an animation event so characters can call target.TakeDamage() at the exact moment
    public bool animationDamageTime = false;

    private IEnumerator TimeStop(float duration)
{
    Time.timeScale = 0f; // Stops the game time
    yield return new WaitForSecondsRealtime(duration); // Waits in real time
    Time.timeScale = 1f; // Resumes the game time
}

    public void SlowTime(float newTimeScale)
    {
        Time.timeScale = newTimeScale;  // Adjust the value to what feels right for the effect
        Time.fixedDeltaTime = 0.02f * Time.timeScale;  // Keep physics simulation smooth
    }

    // Function to normalize time
    public void NormalizeTime()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;  // Reset to default fixed delta time
    }


    public void animationDamageTiming()
{
    
    Debug.Log("This is the damage moment");
   
    animationDamageTime = true;

}
//REWORKING TIMING EVENTS

    public bool isWindowOpen = false;
    public bool damageApplied = false; // New flag to track if damage has been applied

    // Called by the animation event to open the timing window
    
    public void OpenTimingWindow()
    {
        isWindowOpen = true;
        damageApplied = false; // Reset the flag when the window opens
        Debug.Log("Timing window opened.");
    }

    // Called by the animation event to close the timing window
    public void CloseTimingWindow()
    {
        isWindowOpen = false;
        Debug.Log("Timing window closed.");
    }

    // Method to check player input
    public bool CheckPlayerInput()
    {
        if (isWindowOpen && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Player clicked within the timing window!");
            return true; // Successful timing
        }
        return false; // Missed timing
    }

//REWORKING TIMING EVENTS
    public EnemyDeathEffect enemyDeathEffect;
   public void CheckForDeath()
{
    Debug.Log("Checking for death");
    if (this.health <= 0)
    {
        // Check if the Character is an Enemy
        if (this is Enemy enemy)
        {
            enemy.enemyDeathEffect.TriggerExplosion();
            StartCoroutine(enemy.FadeOutSprite());
            enemy.DropMaterial(); // Call DropMaterial on the enemy instance
            // Add any additional logic here for when an enemy dies.
        }
        else if (this.gameObject.tag == "Companion")
        {
            StartCoroutine(this.FadeOutSprite());
        }
    }
}


    public IEnumerator MoveToTarget()
    {
        Debug.Log("MOVING IS CALLED");
        animator.SetTrigger("MovementAnimationTrigger");
        this.isMoving = true;

        float moveSpeed = 17f; 
        Vector3 targetPosition = new Vector3(attackTarget.position.x, attackTarget.position.y, attackTarget.position.z);
        checkCollisionsDuringMovement = true;

        Debug.Log($"Moving to target: {targetPosition}");
        Debug.Log($"Current position: {transform.position}");
        Debug.Log($"Original position before moving: {originalPosition}");

        yield return Move(targetPosition, 3.0f, moveSpeed);

        animator.SetTrigger("StopMovementAnimationTrigger");
        this.isMoving = false;
    }

    public IEnumerator ReturnToPosition(float returnSpeed = 20f)
    {
        Debug.Log($"Returning to original position at: {originalPosition}");
        animator.SetTrigger("MovementAnimationTrigger");
        this.isMoving = true;

        yield return Move(originalPosition, 0.00f, returnSpeed);

        transform.position = originalPosition;
        animator.SetTrigger("StopMovementAnimationTrigger");
        this.isMoving = false;
        Debug.Log("Returned to original position");
    }

    private IEnumerator Move(Vector3 targetPosition, float stoppingDistance, float speed)
    {
        Debug.Log($"Starting Move towards {targetPosition}");
        while (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            Debug.Log($"Moving towards {targetPosition} from {transform.position}");
            yield return null;
        }
        Debug.Log("Completed Move");
    }


/*
// Updated to check for an appropriate stopping distance
private bool HasReachedPosition(Vector3 targetPosition, float stoppingDistance)
{
    // Adjusted to use magnitude instead of sqrMagnitude for more accurate comparison
     
    return Vector3.Distance(transform.position, targetPosition) <= stoppingDistance;
}
*/
/*
private bool IsCollidingWithCharacter()
{
    // Consider using a more specific collision check if necessary
    // For example, Physics2D.OverlapCircle might be replaced with Physics2D.OverlapBox if that's more appropriate for your game
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.0f);

    foreach (Collider2D collider in colliders)
    {
        if (collider.gameObject.GetInstanceID() == gameObject.GetInstanceID()) continue;

        if (this.CompareTag("Enemy") && collider.CompareTag("Enemy")) continue;

        if (this.CompareTag("UI")) continue;

        if (collider.gameObject.GetInstanceID() == attackTarget.gameObject.GetInstanceID()) return true;
    }
    return false;
}
*/

        public virtual void UpdateStats()
    {   
        //Each enemy implements its own growth
    }

    public AudioSource audioSource; // Attach this in the inspector
    public AudioClip hitSound; // Assign this in the inspector
    public AudioClip criticalHitSound; // Assign this in the inspector

    public void PlayHitSound()
    {
        audioSource.clip = hitSound;
        audioSource.Play();
        Debug.Log("hit sound");
    }

    public void PlayCriticalHitSound()
    {
        audioSource.clip = criticalHitSound;
        audioSource.Play();
        Debug.Log("crit sound");
    }
    

}