using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Character Spawner { get; set; }
    public Character Target { get; set; }  // Add a target property
    public int damage;
    public bool isColliding;

    void Update()
    {
        // Moves the projectile in the direction it's facing
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile collided");
        if (!isColliding && other.GetComponent<Character>() == Target)  // Check if the collided object is the target
        {
            isColliding = true;
            Target.TakeDamage(damage, Spawner);  // Pass Spawner as the attacker
            Target.CheckForDeath();
            Destroy(gameObject);  // Destroy the projectile on hit
        }
    }




     
}


