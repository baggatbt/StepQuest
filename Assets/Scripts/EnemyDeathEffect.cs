using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyDeathEffect : MonoBehaviour
{
    public ParticleSystem explosionEffect; // Assign your configured Particle System

    // Call this to trigger the "explosion"
    public void TriggerExplosion()
    {
        // You can customize this part to use specific parts of the sprite or generic particles
        var mainModule = explosionEffect.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(GetComponent<SpriteRenderer>().color); // Example: match enemy color

        explosionEffect.Play(); // Start the explosion effect
      //  GetComponent<SpriteRenderer>().enabled = false; // Hide the sprite
        // Additional cleanup or gameplay effects here
    }
}
