using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Stage : MonoBehaviour
{
    public Button stageButton;
    public BattleConfig stageBattleConfig;
    public string battleSceneName = "BattleScene"; 
    public bool isUnlocked;
    public bool isFirstCompletion;



    private void Start()
    {
        stageButton.onClick.AddListener(OnStageButtonClicked);
        UpdateButtonColor();
    }

    public void UpdateButtonColor()
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



    private void OnStageButtonClicked()
    {
        if (isUnlocked)
        {
        GameManager.Instance.CurrentBattleConfig = stageBattleConfig; // Store the config in the GameManager
        SceneManager.LoadScene(battleSceneName); // Load the battle scene
        }
    }
}
