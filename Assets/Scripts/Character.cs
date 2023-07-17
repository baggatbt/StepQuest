using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour 
{
    public int health;
    public int maxHealth;
    public int damage;
    public Slider healthBar;
    public Transform attackTarget; 
    public Vector3 originalPosition;
    public bool isAttacking = false;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public bool animationEnded;
    public Skill currentSkill; // Property to store the current skill being used by the character
    public List<Skill> skills = new List<Skill>(); //List of the skills available to the character
    public int attacksBeforeSpecial; //Number of attacks that must be used before enemy uses a special
    public Skill normalSkill; //Enemies normal skills
    public Skill specialSkill; //Enemies special skills



    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        if (currentSkill != null)
        {
            currentSkill.skillExecutionComplete = false;
        }


    }


    public void TakeDamage(int damage) 
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0) 
        {
            Destroy(gameObject);
            
        }
    }

     public void AnimationEnded()
    {
        animationEnded = true;
    }


    public IEnumerator MoveToTarget()
{
    originalPosition = transform.position;
   
    Vector3 targetPosition = new Vector3(attackTarget.position.x, transform.position.y, transform.position.z);

    yield return StartCoroutine(Move(targetPosition, 6.0f));



    
}

public IEnumerator ReturnToPosition()
{
    yield return StartCoroutine(Move(originalPosition, 6.0f));
}

 private IEnumerator Move(Vector3 targetPosition, float speed)
{
    
    float stoppingDistance = 0.1f; // Adjust this value to control the stopping distance

    while ((transform.position - targetPosition).sqrMagnitude > stoppingDistance * stoppingDistance)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check for collision with enemy
        Collider2D[] colliders = Physics2D.OverlapCapsuleAll(transform.position, GetComponent<CapsuleCollider2D>().size, GetComponent<CapsuleCollider2D>().direction, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                StopMoving();
                yield break; // Exit the coroutine
            }
        }

        yield return null;
    }
}


public void StopMoving()
{
    StopAllCoroutines();
}

    

}