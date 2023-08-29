using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource; // Plays sound effects
    public AudioSource musicSource; //Plays music
    public AudioClip slashSound;
    public AudioClip blockSound;
    public AudioClip playerIsHitSound;
    public AudioClip battleMusic;


    void Awake()
    {
        // Make this a singleton
        if (instance == null)
        
        {
            musicSource.loop = true;
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
{
    PlayBattleMusic();
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


    [Range(0f, 1f)]
public float musicVolume = 0.1f; // Default volume, but adjustable in inspector

public void PlayBattleMusic()
{
    if (!musicSource.isPlaying)
    {
        musicSource.clip = battleMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }
}



public void StopBattleMusic()
{
    if (musicSource.isPlaying)
    {
        musicSource.Stop();
    }
}

public void ToggleBattleMusic()
{
    if (musicSource.isPlaying)
    {
        StopBattleMusic();
    }
    else
    {
        PlayBattleMusic();
    }
}



}
