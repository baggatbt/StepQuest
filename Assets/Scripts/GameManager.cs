using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleConfig CurrentBattleConfig { get; set; } // Temporary storage for the battle config

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes sure the GameManager persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
