using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Stage : MonoBehaviour
{
    public Button stageButton;
    public BattleConfig stageBattleConfig;
    public string battleSceneName = "TestPortraitBattle"; 
    public bool isUnlocked;
    public bool isFirstCompletion;
    public string stageID; // Unique identifier for the stage
    public List<Stage> connectedStages; // Stages that get unlocked after completing this one

    private void Start()
    {
         stageButton.onClick.AddListener(OnStageButtonClicked);
        // Initialize the unlocked state based on GameManager's data
        isUnlocked = GameManager.Instance.UnlockedStageNames.Contains(this.stageID);
       // UpdateButtonColor();
        
    }

    
    public void UpdateButtonColor()
{
    // Check if the stageButton is not null before accessing it
    /*
    if (stageButton != null)
    {
        if (isUnlocked)
        {
            stageButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            stageButton.GetComponent<Image>().color = Color.red;
        }
    }
    */
}




    private void OnStageButtonClicked()
    {
        if (isUnlocked)
        {
        GameManager.Instance.CurrentBattleConfig = stageBattleConfig; // Store the config in the GameManager
        SceneManager.LoadScene(battleSceneName); // Load the battle scene
        }
    }
}
