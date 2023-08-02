using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class Character : MonoBehaviour 
{
    public int health;
    public int maxHealth;
    public int damage;
    public int energy;
    public int maxEnergy;
    public Slider healthBar;
    public Slider energyBar;
    public Transform attackTarget;
    public Vector3 originalPosition;
    public bool isAttacking = false;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public TextMeshProUGUI healthText; 
    public TextMeshProUGUI energyText;


    public bool animationEnded;
    public Skill currentSkill; 
    public List<Skill> skills = new List<Skill>();
    public int attacksBeforeSpecial; 
    public Skill normalSkill; 
    public Skill specialSkill;

    private bool checkCollisionsDuringMovement = true;

    protected virtual void Start()
{
    animator = GetComponent<Animator>();
    health = maxHealth;
    if (healthBar != null)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        if (healthText !=null && energyText !=null) //This check can eventually be removed once everyone has a HP bar. For now it stays
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
    originalPosition = transform.position;
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
    Debug.Log("Current Energy: " + energy);  // Add this
    if (energyBar != null)
    {
        energyBar.value = energy;
        energyText.text = "MP: " + energy;
    }
}

    

    public void IsHit() => animator.SetTrigger("IsHurtTrigger");
    public void AnimationEnded() => animationEnded = true;

    public IEnumerator MoveToTarget()
    {
        Vector3 targetPosition = GetTargetPosition(1.0f);
        checkCollisionsDuringMovement = true;
        yield return StartCoroutine(Move(targetPosition, 10.0f));
    }

    public IEnumerator ReturnToPosition()
    {
        checkCollisionsDuringMovement = false;
        yield return StartCoroutine(Move(originalPosition, 10.0f));
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
            if (collider.gameObject.GetInstanceID() != gameObject.GetInstanceID() 
                && (collider.CompareTag("Enemy") || collider.CompareTag("Player")))
            {
                return true;
            }
        }
        return false;
    }
}
