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
            stageBattleConfig.currentParty = new List<Companion>(GameManager.Instance.currentParty);
            GameManager.Instance.CurrentBattleConfig = stageBattleConfig;
            GameManager.Instance.SaveCurrentParty();
            SceneManager.LoadScene(battleSceneName);
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
