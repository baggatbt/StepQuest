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

    public bool animationEnded;
    public Skill currentSkill; // Property to store the current skill being used by the character
    public List<Skill> skills = new List<Skill>(); //List of the skills available to the character
    public int attacksBeforeSpecial; //Number of attacks that must be used before enemy uses a special
    public Skill normalSkill; //Enemies normal skills
    public Skill specialSkill; //Enemies special skills


    private void Awake()
{
    healthBar.fillRect.GetComponent<Image>().color = Color.green;
}


    private void Start()
    {
        animator = GetComponent<Animator>();
        healthBar.fillRect.GetComponent<Image>().color = Color.green;
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
    UpdateHealthBarColor();  // add this line

    if (health <= 0) 
    {
        Destroy(gameObject);
    }
}

    private void UpdateHealthBarColor()
{
    float healthPercentage = (float)health / maxHealth;
    healthBar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.green, healthPercentage);
}



     public void AnimationEnded()
    {
        animationEnded = true;
    }

/*
    public IEnumerator MoveToTarget()
    {
        originalPosition = transform.position;
        isAttacking = true;

        Vector3 targetPosition = new Vector3(attackTarget.position.x, attackTarget.position.y, transform.position.z);

        yield return StartCoroutine(Move(targetPosition, 4f));

        isAttacking = false;
    }





    public IEnumerator ReturnToPosition()
{
    yield return StartCoroutine(Move(originalPosition, 4f));

    isAttacking = false;
}

    private IEnumerator Move(Vector3 targetPosition, float speed)
    {
        while ((transform.position - targetPosition).sqrMagnitude > 0.01f) // using sqrMagnitude for performance reasons
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
    
*/
}

