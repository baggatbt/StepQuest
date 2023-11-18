using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleConfig CurrentBattleConfig { get; set; } // Temporary storage for the battle config

    public int currentStageID = 0; //This will need to be replaced by loading data on start. for testing purposes setting to 0
    public int[] stageIDs = new int[] {0, 1, 2};


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
