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
        if (isUnlocked)
        {
            stageButton.GetComponent<Image>().color = Color.green; // Set color to green if unlocked
        }
        else
        {
            stageButton.GetComponent<Image>().color = Color.red; // Set color to red if locked
        }
    }

    private void OnStageButtonClicked()
    {
        if (stageButton.isUnlocked)
        {
        GameManager.Instance.CurrentBattleConfig = stageBattleConfig; // Store the config in the GameManager
        SceneManager.LoadScene(battleSceneName); // Load the battle scene
        }
    }
}
