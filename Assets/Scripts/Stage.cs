using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage : MonoBehaviour
{
    public Button stageButton;
    public string battleSceneName; 
    public bool isUnlocked;
    public bool isBossBattle;
    public BattleConfig stageBattleConfig;
    public string stageID; // Unique identifier for the stage
    public List<Stage> connectedStages; // Add this line to define connected stages

    private void Start()
    {
         stageButton.onClick.AddListener(OnStageButtonClicked);
        // Initialize the unlocked state based on GameManager's data
        isUnlocked = GameManager.Instance.UnlockedStageNames.Contains(this.stageID);
       // UpdateButtonColor();
        
    }

    private void OnStageButtonClicked()
    {
        if (isUnlocked)
        {
        GameManager.Instance.CurrentBattleConfig = stageBattleConfig; // Store the config in the GameManager
        SceneManager.LoadScene(battleSceneName); // Load the battle scene
        }
    }



    public void Configure(StageConfig config)
    {
        this.stageID = config.stageID;
        this.battleSceneName = config.battleSceneName;
        this.isUnlocked = config.isUnlocked;

        UpdateButtonAppearance();
    }

    public void UpdateButtonAppearance()
    {
        if (stageButton != null)
        {
            stageButton.interactable = isUnlocked;
            stageButton.GetComponent<Image>().color = isUnlocked ? Color.green : Color.red;
        }
    }

}
