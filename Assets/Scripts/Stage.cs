using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Stage : MonoBehaviour
{
    public Button stageButton;
    public BattleConfig stageBattleConfig;
    public string battleSceneName = "BattleScene"; // Name of your battle scene
    public bool isUnlocked;

    private void Start()
    {
        stageButton.onClick.AddListener(OnStageButtonClicked);
    }

    private void OnStageButtonClicked()
    {
        GameManager.Instance.CurrentBattleConfig = stageBattleConfig; // Store the config in the GameManager
        SceneManager.LoadScene(battleSceneName); // Load the battle scene
    }
}
