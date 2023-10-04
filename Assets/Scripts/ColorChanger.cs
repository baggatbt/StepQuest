using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorChanger : MonoBehaviour
{
    new public Renderer renderer;
    public float duration;  // Duration for the transition

    private float timer = 0.0f;
    private bool isTiming = false;

    void Update()
    {
        if (isTiming)
        {
            // Increment the timer by delta time
            timer += Time.deltaTime;

            // Calculate the interpolation factor
            float t = timer / duration;

            // Change the color of the renderer
            renderer.material.color = Color.Lerp(Color.red, Color.green, t);

            // Stop timing once the transition is complete
            if (t >= 1.0f)
            {
                isTiming = false;
                timer = 0.0f;
            }
        }
    }

    // Call this function to start the color transition
    public void StartColorTransition(float durationOfWindow)
    {
        duration = durationOfWindow;
        timer = 0.0f;
        isTiming = true;
    }
}
