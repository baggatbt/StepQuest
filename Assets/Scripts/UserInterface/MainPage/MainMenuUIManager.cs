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

    //KNIGHT 
    public Knight knight;
    public TextMeshProUGUI knightHPText;
    public TextMeshProUGUI knightMPText;
    public TextMeshProUGUI knightAtkText;
    public TextMeshProUGUI knightDefText;
    public TextMeshProUGUI knightSpdText;
    public TextMeshProUGUI knightSPText;
    public Button knightHPBtn;
    public Button knightMPBtn;
    public Button knightATKBtn;
    public Button knightDEFBtn;
    public Button knightSPDBtn;

    //ARCHER
    public Archer archer;
    public TextMeshProUGUI archerHPText;
    public TextMeshProUGUI archerMPText;
    public TextMeshProUGUI archerAtkText;
    public TextMeshProUGUI archerDefText;
    public TextMeshProUGUI archerSpdText;
    public TextMeshProUGUI archerSPText;
    public Button archerHPBtn;
    public Button archerMPBtn;
    public Button archerATKBtn;
    public Button archerDEFBtn;
    public Button archerSPDBtn;
    
  

    private void Start()
    {
        knight = GameManager.Instance.knight;
        archer = GameManager.Instance.archer;
        UpdateUI();
    }
    private void Update()
    {
         if (stepsText != null) stepsText.text =  PlayerData.Instance.inGameSteps.ToString();  
    }
    private void UpdateUI()
    {  
       

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
        GameManager.Instance.PopulateCompanionList();
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
