using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioClip slashSound;
    public AudioClip blockSound;
    public AudioClip playerIsHitSound;

    void Awake()
    {
        // Make this a singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySlashSound()
    {
        audioSource.PlayOneShot(slashSound);
    }

    public void PlayBlockSound()
    {
        audioSource.PlayOneShot(blockSound);
    }

    public void PlayPlayerIsHitSound()
    {
        audioSource.PlayOneShot(playerIsHitSound);
    }
}
