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
    public Vector3 currentPosition;

    public bool isHit; //Used to track when a character takes a hit

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
        currentPosition = transform.position;
        originalPosition = currentPosition; // Add this line to store the original position

        Debug.Log(currentPosition);
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


    public void AnimationTimingEvent()
    {
        Debug.Log("This was the timing event");
    }
     
}

