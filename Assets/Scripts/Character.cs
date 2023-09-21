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
    public int maxEnergy;
    public int attackPower;
    public int defensePower;
    public int speed;
    
    public Slider healthBar;
    public Slider energyBar;
    public Transform attackTarget;
    public Vector3 originalPosition;
    public bool isAttacking = false;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public TextMeshProUGUI healthText; 
    public TextMeshProUGUI energyText;
     


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
        Debug.Log(gameObject.name + " original position: " + originalPosition);
        
    }

    void Update(){

    }

    protected virtual void Start()
    {
        
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
            energyBar.maxValue = maxEnergy;
            energyBar.value = energy;
        }
    }

    

    public CameraShake cameraShake; // Reference to the CameraShake script

    public void TakeDamage(int damageOfAttacker, Character attacker)
{
    int damageDealt = damageOfAttacker * (1 - (this.defensePower / 100));

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
    Debug.Log(attacker.isAttacking);
        
}   

    public void CheckForDeath()
    {
        if (health <= 0)
        {
            StartCoroutine(FadeOutSprite());
        }
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


    public void SpendEnergy(int energySpent)
{
    energy -= energySpent;
   // Debug.Log("Current Energy: " + energy); 
    if (energyBar != null)
    {
        energyBar.value = energy;
        energyText.text = "MP: " + energy;
    }
}

    //This will be called on an animation event so characters can call target.TakeDamage() at the exact moment
    public void animationDamageTiming()
    {
        Debug.Log("This is the damage moment");
        
    }

    public void GainEnergy(int energyGained)
{
    if (energyGained % 2 != 0) //Checks to make sure energy values stay rounded 
    {
        energyGained--;
    }

    if (energy + energyGained > maxEnergy) // If the gained energy will bring the total over the max
    {
        energy = maxEnergy; // Set energy to the max
    }
    else // If the gained energy will not bring the total over the max
    {
        energy += energyGained; // Add the gained energy to the total
    }
    
    //Debug.Log("Current Energy: " + energy); 
    
    if (energyBar != null)
    {
        energyBar.value = energy;
        energyText.text = "MP: " + energy;
    }
}

   
    

    public void IsHit() => animator.SetTrigger("IsHurtTrigger");
    
    public bool isAnimationDone = false;

    //Called at points in an animation to flag that its over and the animation can continue or stop
    public void AnimationEnded()
    {
        isAnimationDone = true;
    }

     public IEnumerator MoveToTarget()
{
    Vector3 targetPosition = attackTarget.position;
    checkCollisionsDuringMovement = true;
    animator.SetTrigger("MovementAnimationTrigger");  
    yield return Move(targetPosition);
    animator.SetTrigger("StopMovementAnimationTrigger");  
}

     public IEnumerator ReturnToPosition()
        {
            yield return new WaitForSeconds(0.5f);
            checkCollisionsDuringMovement = false;
            animator.SetTrigger("MovementAnimationTrigger");  
            yield return Move(originalPosition);
            animator.SetTrigger("StopMovementAnimationTrigger");  
        }

     private IEnumerator Move(Vector3 targetPosition)
        {
            while (!HasReachedPosition(targetPosition))
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 15.0f * Time.deltaTime);
                
                if (checkCollisionsDuringMovement && IsCollidingWithCharacter())
                {
                    animator.SetTrigger("StopMovementAnimationTrigger");  
                    yield break;
                }
                yield return null;
            }
        }


    private bool HasReachedPosition(Vector3 targetPosition, float stoppingDistance = 2.0f)
    {
        return (transform.position - targetPosition).sqrMagnitude <= stoppingDistance * stoppingDistance;
    }

    private bool IsCollidingWithCharacter()
{
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);

    foreach (Collider2D collider in colliders)
    {
        if (collider.gameObject.GetInstanceID() == gameObject.GetInstanceID()) continue;

        // If current object is an enemy and the colliding object is also an enemy, ignore the collision
        if (this.CompareTag("Enemy") && collider.CompareTag("Enemy")) continue;

        // If it's colliding with the target, then return true
        if (collider.gameObject.GetInstanceID() == attackTarget.gameObject.GetInstanceID()) return true;
    }
    return false;
}



}
