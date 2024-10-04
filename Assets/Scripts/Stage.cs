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
    public string stageID;
    public List<Stage> connectedStages;

    private void Start()
    {
        stageButton.onClick.AddListener(OnStageButtonClicked);
        isUnlocked = GameManager.Instance.UnlockedStageNames.Contains(this.stageID);
    }

    private void OnStageButtonClicked()
{
    if (isUnlocked)
    {
        // Store the current stage in GameManager
        GameManager.Instance.CurrentStage = this;

        stageBattleConfig.currentParty = new List<Companion>(GameManager.Instance.currentParty);
        GameManager.Instance.CurrentBattleConfig = stageBattleConfig;
        GameManager.Instance.SaveCurrentParty();
       // SceneManager.LoadScene(battleSceneName);
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

    // Method to get the next stage in the series
    public Stage GetNextStage()
    {
        
        if (connectedStages != null && connectedStages.Count > 0)
        {
            // Picks the first connected stage of which each stage has one
            return connectedStages[0]; // Or use Random.Range(0, connectedStages.Count) for random stage selection
        }

        // If no connected stages, then go back to town. TODO
        return null;
    }
}
