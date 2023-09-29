using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Character Spawner { get; set; }
    public int damage;
    public bool isColliding;

    void Update()
    {
        // Moves the projectile in the direction it's facing
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Ive been collided");
        Character character = other.GetComponent<Character>();
        if (character != null && character != Spawner)
        {
            isColliding = true;
            Destroy(gameObject);  // destroy the projectile on hit
            
        }
    }
}


