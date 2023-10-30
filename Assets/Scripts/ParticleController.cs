using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleController : MonoBehaviour
{
    public ParticleSystem particleSystem;

    public void PlayParticleEffect()
    {
        particleSystem.Play();
        StartCoroutine(StopParticleAfterDuration());
    }

    private IEnumerator StopParticleAfterDuration()
    {
        // Wait for the duration of the particle system
        yield return new WaitForSeconds(particleSystem.main.duration);

        // Stop the particle system
        particleSystem.Stop();
    }
}
