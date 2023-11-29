using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class Character : MonoBehaviour 
{
    public int level;
    public int health;
    public int maxHealth;
    public int damage; //This is for the slime, so the next time you forget and wonder, what is this for again? Thats what. Everyone else has converted to AP
    public int energy;
    public int maxEnergy = 5;
    public int teamEnergy;
    public int attackPower;
    public int defensePower;
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
        animator = GetComponent<Animator>();
        health = maxHealth; 
        originalPosition = transform.position;
        statusEffectController = GetComponent<StatusEffectController>();
        this.hasNotGone = true;
        Debug.Log(gameObject.name + " original position: " + originalPosition);
        if (healthText != null)
    {
        healthText.text = "HP: " + health;
        Debug.Log(health);
    }
    else 
    {
        Debug.Log("Healtexh is null");
    }
        
    }

    void Update(){
        if (this.healthBar != null)
        {
            this.healthBar.maxValue = maxHealth;
            this.healthBar.value = health;
            if (this.healthText != null )
            {
                this.healthText.text = "HP: " + this.health; 
                
            }
            if (this.energyText != null)
            {
                
                this.energyText.text = "MP: " + PlayerData.Instance.teamEnergy + " / " + maxEnergy;
                this.energyBar.value = PlayerData.Instance.teamEnergy;
                
                
            }
        }
        if (energyBar != null)
        {
            
            
            energyBar.maxValue = maxEnergy;
            energyBar.value = PlayerData.Instance.teamEnergy;
        }
        

    }

    protected virtual void Start()
    {
        /*
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
            if (healthText !=null && energyText !=null)
            {
                healthText.text = "HP: " + health; 
                energyText.text = "MP: " + energy;
            }
        }
        

        if (energyBar != null)
        {
            energyBar.maxValue = maxenergy;
            energyBar.value = energy;
        }
        */
    }

    

    public CameraShake cameraShake; // Reference to the CameraShake script

    public void TakeDamage(int damageOfAttacker, Character attacker)
{
    
   // Define a constant that will be used to adjust the effectiveness of defense.
    const float defenseEffectiveness = 50.0f; // This is a balancing factor.
    
    // Calculate damage reduction using a diminishing returns formula.
    float damageReduction = this.defensePower / (this.defensePower + defenseEffectiveness);
    int damageDealt = (int)(damageOfAttacker * (1 - damageReduction));
    
    Debug.Log("Damage reduced by Defense: " + (damageOfAttacker - damageDealt));

    health -= damageDealt;
    healthBar.value = health;

    // Update the health text.
    if (healthText != null)
    {
        healthText.text = "HP: " + health;
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
}

    private int remainingCost = 0;
    public void SpendEnergy(int energySpent)
    {
  
        PlayerData.Instance.teamEnergy -= energySpent;
        
        
        energyBar.value = PlayerData.Instance.teamEnergy;
        if (energyBar != null)
        {
            energyBar.value = PlayerData.Instance.teamEnergy;
            energyText.text = "MP: " + PlayerData.Instance.teamEnergy + " / " + maxEnergy;
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
        energyText.text = "MP: " + PlayerData.Instance.teamEnergy + " / " + maxEnergy;
    }
    Debug.Log("Not passing any");
}
    //For enemy rage bars
    public void GainEnergy(int energyGained)
    {
        this.energy = energyGained;
        
    }

   
    public void IsHit() => animator.SetTrigger("IsHurtTrigger");
    
    public bool isAnimationDone = false;

    //Called at points in an animation to flag that its over and the animation can continue or stop
    public void AnimationEnded()
    {
         Debug.Log("animation ended!");
         isAnimationDone = true;
        
    }

    //This will be called on an animation event so characters can call target.TakeDamage() at the exact moment
    public bool animationDamageTime = false;

    public void animationDamageTiming()
    {
        Debug.Log("This is the damage moment");
        //animator.SetTrigger("TimingFlashTrigger");
        animationDamageTime = true;;
    
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

    Vector3 targetPosition = attackTarget.position;
    checkCollisionsDuringMovement = true;

    // Start moving towards the target
    yield return Move(targetPosition, stoppingDistance: 0.1f); // Set a small stopping distance

    // Stop the movement animation when the target position is reached or a collision occurs
    animator.SetTrigger("StopMovementAnimationTrigger");
    this.isMoving = false;
}

public IEnumerator ReturnToPosition()
{
    animator.SetTrigger("MovementAnimationTrigger");
    this.isMoving = true;

    // Move back to the original position
    yield return Move(originalPosition, stoppingDistance: 0.0f); // Exact position, so stopping distance is 0

    // This line will ensure the character is exactly at the original position
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

}