using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class Character : MonoBehaviour 
{
    public int health;
    public int maxHealth;
    public int damage; //This is for the slime, so the next time you forget and wonder, what is this for again? Thats what.
    public int energy;
    public int maxEnergy;
    public int attackPower;
    public int defensePower;
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

    public bool IsAffectedBy(string effectName)
    {
        return statusEffectController.HasEffect(effectName);
    }

    protected virtual void Awake()
{
    animator = GetComponent<Animator>();
    health = maxHealth; 
    originalPosition = transform.position;
    statusEffectController = GetComponent<StatusEffectController>();
    Debug.Log(gameObject.name + " original position: " + originalPosition);
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
            energyText.text = "HP: " + energy;
        }
    }

    if (energyBar != null)
    {
        energyBar.maxValue = maxEnergy;
        energyBar.value = energy;
    }
}



    public void TakeDamage(int damage)
{
    health -= damage;
    healthBar.value = health;

    // Update the health text.
    if (healthText != null)
    {
        healthText.text = "HP: " + health;
    }

    if (damage > 0) IsHit();
    if (health <= 0) this.gameObject.SetActive(false);
}


    public void SpendEnergy(int energySpent)
{
    energy -= energySpent;
    Debug.Log("Current Energy: " + energy); 
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
    
    Debug.Log("Current Energy: " + energy); 
    
    if (energyBar != null)
    {
        energyBar.value = energy;
        energyText.text = "MP: " + energy;
    }
}


    

    public void IsHit() => animator.SetTrigger("IsHurtTrigger");
    public void AnimationEnded() => animationEnded = true;
    public void attackStageTrigger() => attackTrigger = true;

    public IEnumerator MoveToTarget()
    {
        Vector3 targetPosition = GetTargetPosition(1.0f);
        checkCollisionsDuringMovement = true;
        yield return StartCoroutine(Move(targetPosition, 15.0f));
    }

    public IEnumerator ReturnToPosition()
    {
        Debug.Log("Is returning to position");
        checkCollisionsDuringMovement = false;
        yield return StartCoroutine(Move(originalPosition, 15.0f));
        animator.SetTrigger("StopMovementAnimationTrigger");
    }

    public void StopMoving()
    {
        animator.SetTrigger("StopMovementAnimationTrigger");
        checkCollisionsDuringMovement = false;
    }

    private Vector3 GetTargetPosition(float offset)
    {
        Vector3 direction = (attackTarget.position - transform.position).normalized;
        return attackTarget.position - direction * offset;
    }

    private IEnumerator Move(Vector3 targetPosition, float speed)
    {
        Debug.Log(gameObject.name + " is trying to move to: " + targetPosition);
    
        animator.SetTrigger("MovementAnimationTrigger");
        while (!HasReachedPosition(targetPosition))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            
            if (checkCollisionsDuringMovement && IsCollidingWithCharacter())
            {
                StopMoving();
                yield break;
            }

            yield return null;
        }
    }

    private bool HasReachedPosition(Vector3 targetPosition, float stoppingDistance = 0.1f)
    {
        return (transform.position - targetPosition).sqrMagnitude <= stoppingDistance * stoppingDistance;
    }

    private bool IsCollidingWithCharacter()
{
    Collider2D[] colliders = Physics2D.OverlapCapsuleAll(
        transform.position, 
        GetComponent<CapsuleCollider2D>().size, 
        GetComponent<CapsuleCollider2D>().direction, 
        0f
    );

    foreach (Collider2D collider in colliders)
    {
        // Ignore the collider if it's the current object itself.
        if (collider.gameObject.GetInstanceID() == gameObject.GetInstanceID()) 
        {
            continue;
        }

        // If the colliding objects have the same tag, ignore the collision.
        if (collider.CompareTag(tag))
        {
            continue;
        }

        // If collider has a tag "Enemy" or "Player", then we've found a collision.
        if (collider.CompareTag("Enemy") || collider.CompareTag("Player"))
        {
            return true;
        }
    }
    return false;
}

}
