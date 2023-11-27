using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleConfig CurrentBattleConfig { get; set; } // Temporary storage for the battle config

    public int currentStageID = 1; //This will need to be replaced by loading data on start. for testing purposes setting to 0
    
    //CURRENT ISSUE, STAGE ID ALWAYS INCREMENTS AFTER EVERY WIN EVEN IF REPLAYING STAGE.
    //No replaying of old stages, spawn new ones?
    //Use battle config to grab the stage ID, then use

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

    public void UnlockNextStage()
    {
        if (CurrentBattleConfig.stageID > currentStageID)
        {
            currentStageID++;
        }
    }

    
}
