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
    
     

    public bool hasNotGone;
    public bool isMoving;
    public bool attackTrigger;
    public bool animationEnded;
    public Skill currentSkill; 
    public List<Skill> skills = new List<Skill>();
    public int attacksBeforeSpecial; 
    public Skill normalSkill; 
    public Skill specialSkill;

    private bool checkCollisionsDuringMovement = true;

    //STATUS EFFECTS
    public StatusEffectController statusEffectController;

    

    protected virtual void Awake()
    {
        isSelected = false;
        animator = GetComponent<Animator>();
        statusEffectController = GetComponent<StatusEffectController>();
        originalPosition = transform.position;
        Debug.Log("OP at Awake() " + originalPosition);
       // this.hasNotGone = true;
    }

    /*
    void Update(){
            
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
    */

    protected virtual void Start()
    {
        
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
            if (healthText !=null && energyText !=null)
            {
               this.healthText.text = this.health + "";
                energyText.text = this.energy  + "";
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
    // Calculate effective defense after penetration
    int effectiveDefense = Math.Max(0, defensePower - attacker.defensePenetration);
    Debug.Log("The target has " + effectiveDefense + " defense left after penetration.");

    // Calculate total damage using the new formula
    float totalDamage = damageOfAttacker * (100f / (100f + effectiveDefense));
    int damageDealt = Mathf.FloorToInt(totalDamage); // Convert to integer, adjust as needed

    Debug.Log("Total damage dealt after defense penetration = " + damageDealt);

    // Subtract the calculated damage from health
    this.health -= damageDealt;
    this.healthBar.value = health;
    this.EnemyIsHit(); //If the character taking damage is an enemy, they all share the same trigger
    Debug.Log("Takign damage but im null" + this.healthText);
    // Update the health text
    if (this.healthText != null)
    {
        this.healthText.text = this.health + "";
    }
    
    // Check if damage was dealt for additional effects
    if (damageDealt > 0)
    { 
        // Trigger hit reaction, damage popup, etc.
        GameObject damagePopupPrefab = Resources.Load<GameObject>("PreFab/DamagePopup");
        Transform endOfBattleRewardsTransform = GameObject.Find("EndOfBattleRewardsCanvas").transform;
        if(damagePopupPrefab != null)
        {
            GameObject damagePopupInstance = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity, endOfBattleRewardsTransform);
            DamagePopup damagePopupScript = damagePopupInstance.GetComponent<DamagePopup>();
            damagePopupScript.Setup(damageDealt);
        }
        else
        {
            Debug.LogError("Failed to load DamagePopup prefab.");
        }
    }
    Debug.Log("isAttacking = " + attacker.isAttacking);
   
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
    //For enemy rage bars
    public void GainEnergy(int energyGained)
    {
        this.energy = energyGained;
        
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

    //This will be called on an animation event so characters can call target.TakeDamage() at the exact moment
    public bool animationDamageTime = false;

    private IEnumerator TimeStop(float duration)
{
    Time.timeScale = 0f; // Stops the game time
    yield return new WaitForSecondsRealtime(duration); // Waits in real time
    Time.timeScale = 1f; // Resumes the game time
}


    public void animationDamageTiming()
{
    Debug.Log("This is the damage moment");
    StartCoroutine(TimeStop(0.1f)); // Adjust the duration for timestop effect
    animationDamageTime = true;
}


   public void CheckForDeath()
{
    Debug.Log("Checking for death");
    if (this.health <= 0)
    {
        
        StartCoroutine(FadeOutSprite());
        
      
    }
}


     public IEnumerator MoveToTarget()
{
    animator.SetTrigger("MovementAnimationTrigger");
    this.isMoving = true;

    // Adjust the target position to include the target's Y level
    Vector3 targetPosition = new Vector3(attackTarget.position.x, attackTarget.position.y, attackTarget.position.z);
    checkCollisionsDuringMovement = true;

    // Start moving towards the adjusted target position
    yield return Move(targetPosition, stoppingDistance: 0.0f); // Use the adjusted target position

    // Stop the movement animation when the target position is reached or a collision occurs
    animator.SetTrigger("StopMovementAnimationTrigger");
    this.isMoving = false;
}


public IEnumerator ReturnToPosition()
{
    animator.SetTrigger("MovementAnimationTrigger");
    this.isMoving = true;

    Debug.Log("Returning to OP at : " + originalPosition);
    // Move back to the exact original position, including Y level
    yield return Move(originalPosition, stoppingDistance: 0.0f); // Ensure originalPosition has the correct Y value

    // This line will ensure the character is exactly at the original position, including Y level
    transform.position = originalPosition;

    animator.SetTrigger("StopMovementAnimationTrigger");
    this.isMoving = false;
}


private IEnumerator Move(Vector3 targetPosition, float stoppingDistance)
{
    while (!HasReachedPosition(targetPosition, stoppingDistance))
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 17.5f * Time.deltaTime);

        if (checkCollisionsDuringMovement && IsCollidingWithCharacter())
        {
            animator.SetTrigger("StopMovementAnimationTrigger");
            yield break; // Stop the coroutine if a collision is detected
        }

        // Use WaitForFixedUpdate for physics-based movement
        yield return new WaitForFixedUpdate();
    }
}

// Updated to check for an appropriate stopping distance
private bool HasReachedPosition(Vector3 targetPosition, float stoppingDistance)
{
    // Adjusted to use magnitude instead of sqrMagnitude for more accurate comparison
    
    return Vector3.Distance(transform.position, targetPosition) <= stoppingDistance;
}

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


        public virtual void UpdateStats()
    {   
        //Each enemy implements its own growth
    }
    

}