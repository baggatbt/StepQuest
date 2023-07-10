using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Skill currentSkill; // Property to store the current skill being used by the character
    public List<Skill> skills = new List<Skill>(); //List of the skills available to the character
    public int attacksBeforeSpecial; //Number of attacks that must be used before enemy uses a special
    public Skill normalSkill; //Enemies normal skills
    public Skill specialSkill; //Enemies special skills



    private void Start()
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

public IEnumerator MoveToTarget()
{
    originalPosition = transform.position;
    isAttacking = true;

    float offset = 1.5f;
    Vector3 adjustedTarget = attackTarget.position - (transform.position - attackTarget.position).normalized * (GetComponent<CapsuleCollider2D>().size.x / 2 - offset);

    yield return StartCoroutine(Move(adjustedTarget, 4f));

    isAttacking = false;
}

public IEnumerator ReturnToPosition()
{
    yield return StartCoroutine(Move(originalPosition, 4f));

    isAttacking = false;
}

private IEnumerator Move(Vector3 targetPosition, float speed)
{
    while ((transform.position - targetPosition).sqrMagnitude > Mathf.Epsilon)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        yield return null;
    }
}
}

