using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenuUIManager : MonoBehaviour
{
    public GameObject knightSkillPanel;
    public GameObject wizardSkillPanel;
    public GameObject archerSkillPanel;
    public GameObject adventurerSkillPanel;
    public GameObject forestTownPanel;
    public GameObject missionGUI;
    public GameObject companionGUI;
    public GameObject characterGUI;
    public GameObject overworldMapGUI;
    public GameObject knightSkillListView;
    public TextMeshProUGUI stepsText;
    public GameObject companionButtonPrefab;
    public GameObject companionListPanel;
    public GameObject companionStatsPanel;
    
    // References to stat UI Text elements
    public TextMeshProUGUI levelText,atkText, hpText, defText, expText, spdText; 

    //KNIGHT 
    //public Knight knight;
    

    //ARCHER
    //public Archer archer;
    
    
  

    private void Start()
    {
       // knight = GameManager.Instance.knight;
       // archer = GameManager.Instance.archer;
       UpdateTimerDisplay(); // Initial display update
        UpdateUI();
    }
    private void Update()
    {
         if (stepsText != null) stepsText.text =  PlayerData.Instance.inGameSteps.ToString();  
         UpdateTimerDisplay();
         UpdateKnightRecoveryTimerDisplay();
    }
    private void UpdateUI()
    {  
       

    }
    public void PopulateCompanionList()
{
    GameObject companionButtonContainer = GameObject.Find("Companion Button Container"); // Find or reference directly

    foreach (Companion companion in GameManager.Instance.companions)
    {
        if (companion.isUnlocked)
        {
            GameObject buttonObject = Instantiate(companionButtonPrefab, companionButtonContainer.transform); // Use the container as parent
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = companion.heroID;
            Button btn = buttonObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate { OnCompanionSelected(companion); });
        }
    }
}


    void OnCompanionSelected(Companion companion)
    {
        // Assuming your Companion class has these fields or properties
        levelText.text = "Lv: " + companion.heroLevel.ToString();
        atkText.text = "ATK: " + companion.attackPower.ToString();
        hpText.text = "HP: " + companion.health.ToString();
        defText.text = "DEF: " + companion.defensePower.ToString();
        expText.text = "EXP: " + companion.heroExp.ToString() + " / " + companion.ExpToNextLevel(companion.heroLevel);
        spdText.text = "SPD: " + companion.speed.ToString();
        
        // Make sure the stats panel is visible
        companionStatsPanel.SetActive(true);
    }
   
    
    public CanvasGroup currentPanel;
    public CanvasGroup ForestMapCanvasGroup;

   public void SwitchPanel(CanvasGroup newPanel)
    {
        // Hide the current panel, if it exists
        if (currentPanel != null)
        {
            currentPanel.alpha = 0;
            currentPanel.interactable = false;
            currentPanel.blocksRaycasts = false;
        }

        // Show the new panel
        newPanel.alpha = 1;
        newPanel.interactable = true;
        newPanel.blocksRaycasts = true;

        // Update the current panel reference to the new panel
        currentPanel = newPanel;
    }

   public void TogglePanel(GameObject panel)
{
    panel.SetActive(!panel.activeSelf);
}

//TIMER MANAGEMENT
        public TMP_Text timerText; // Assign in the inspector
        public float timerDuration = 120; // Duration in seconds, adjust as needed

        private string timerId = "upgradeTimer"; // Unique ID for the timer
        public TMP_Text knightRecoveryText;
        private string knightRecoveryTimerId = "knightRecoveryTimer"; // Unique ID for the knight's recovery timer


       
        

        // Method to start the timer
        public void StartTimer(string timerId, float durationInSeconds)
        {
            TimerManager.Instance.SetTimer(timerId, durationInSeconds);
        }

        // Method to update the timer display
        void UpdateTimerDisplay()
        {
            TimeSpan remainingTime = TimerManager.Instance.GetRemainingTime(timerId);
            if (remainingTime > TimeSpan.Zero)
            {
                timerText.text = string.Format("{0:D2}:{1:D2}", remainingTime.Minutes, remainingTime.Seconds);
            }
            else
            {
                timerText.text = "00:00";
            }
        }

        void OnEnable() {
            TimerManager.Instance.OnTimerCompleted += HandleTimerCompletion;
        }

        void OnDisable() {
            TimerManager.Instance.OnTimerCompleted -= HandleTimerCompletion;
        }

        private void HandleTimerCompletion(string timerId) {
            // Check if the completed timer is the one we're interested in
            if (timerId == "knightRecoveryTimer") { 
                GameManager.Instance.knight.stamina += 1; // Increase stamina
                //update UI or do any other necessary actions
            }
        }

        // Example method call to start the knight's recovery timer
        public void StartKnightRecoveryTimer()
        {
            float knightRecoveryDuration = 600; // For example, 120 seconds for recovery
            StartTimer(knightRecoveryTimerId, knightRecoveryDuration);
        }

        void UpdateKnightRecoveryTimerDisplay() 
        {
            TimeSpan remainingTime = TimerManager.Instance.GetRemainingTime(knightRecoveryTimerId);
            if (remainingTime > TimeSpan.Zero) {
                knightRecoveryText.text = string.Format("{0:D2}:{1:D2}", remainingTime.Minutes, remainingTime.Seconds);
            } else {
                knightRecoveryText.text = "00:00";
                // Additional actions to take when the knight's recovery timer finishes could also be triggered here if needed
            }
        }


        


    











    //OLD NEED TO REDO WITH NEW TECHNIQUE 
        public void openMissionInterface()
        {
            missionGUI.SetActive(true);
        }

        public void closeMissionInterface()
        {
            missionGUI.SetActive(false);
        }

    public void openCompanionInterface()
    {
        companionGUI.SetActive(true);
    }

    public void closeCompanionInterface()
    {
        companionGUI.SetActive(false);
    }

    public void openCharacterInterface()
    {
        characterGUI.SetActive(true);
        PopulateCompanionList();
    }

    public void closeCharacterInterface()
    {
        characterGUI.SetActive(false);
    }

    public void openOverworldMapInterface()
    {
        overworldMapGUI.SetActive(true);
    }

    public void closeOverworldMapInterface()
    {
        overworldMapGUI.SetActive(false);
        Debug.Log("ACTIVATED");
    }

    public void openForestTownPanel()
    {
        forestTownPanel.SetActive(true);
    }

    public void closeForestTownPanel()
    {
        forestTownPanel.SetActive(false);
    }

    public void openKnightSkillTree()
    {
        
        knightSkillListView.SetActive(true);
    }

    
    public void openKnightSkillList()
    {
        knightSkillListView.SetActive(true);
    }
    public void closeKnightSkillList()
    {
        knightSkillListView.SetActive(false);
    }
   
}
