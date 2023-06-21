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
    public Skill currentSkill; // Property to store the current skill being used by the character


    private void Start()
    {
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
        Debug.Log("I am moving");
        originalPosition = transform.position;
        isAttacking = true;

        float speed = 4.0f;
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        float offset = 1.5f;
        Vector3 adjustedTarget = attackTarget.position - (transform.position - attackTarget.position).normalized * (GetComponent<CapsuleCollider2D>().size.x / 2 - offset);

        float journeyLength = Vector3.Distance(startPosition, adjustedTarget);

        while (transform.position != adjustedTarget)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(startPosition, adjustedTarget, fractionOfJourney);

            yield return null;
        }

        Debug.Log("DEBUG 1 - Enemy reached target");

        isAttacking = false;
    }


    public IEnumerator ReturnToPosition()
    {
        float speed = 4.0f;
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, originalPosition);

        while (transform.position != originalPosition)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, originalPosition, fractionOfJourney);

            yield return null;
        }

        isAttacking = false;
    }
}
