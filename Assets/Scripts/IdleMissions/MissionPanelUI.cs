/*
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MissionPanelUI : MonoBehaviour
{
    [Header("Stage Selection")]
    public TMP_Dropdown stageDropdown;

    [Header("Companion Selection")]
    public Transform companionSelectionContainer;
    public GameObject companionTogglePrefab;

    [Header("Mission Execution")]
    public Button sendMissionButton;

    [Header("Active Missions List")]
    public Transform missionListContainer;
    public GameObject missionEntryPrefab;

    private List<Companion> selectedCompanions = new();

    void Start()
    {
        PopulateStageDropdown();
        PopulateCompanionSelection();

        sendMissionButton.onClick.AddListener(OnSendMissionClicked);
        RefreshMissionList();
    }

    void PopulateStageDropdown()
{
    stageDropdown.ClearOptions();

    var unlockedStages = GameManager.Instance.allStagesData
        .Where(s => s.isUnlocked)  // ← Use this!
        .ToList();

    Debug.Log($"Found {unlockedStages.Count} unlocked stages");

    List<string> options = unlockedStages.Select(s => s.stageID).ToList();
    stageDropdown.AddOptions(options);
}


    void PopulateCompanionSelection()
    {
        foreach (Transform child in companionSelectionContainer)
            Destroy(child.gameObject);

        foreach (var companion in GameManager.Instance.currentParty)
        {
            GameObject toggleObj = Instantiate(companionTogglePrefab, companionSelectionContainer);
            toggleObj.GetComponentInChildren<TextMeshProUGUI>().text = companion.heroID;

            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) selectedCompanions.Add(companion);
                else selectedCompanions.Remove(companion);
            });
        }
    }

    void OnSendMissionClicked()
    {
        if (selectedCompanions.Count == 0) return;

        string selectedStageID = stageDropdown.options[stageDropdown.value].text;
        var selectedStage = GameManager.Instance.allStagesData.Find(s => s.stageID == selectedStageID);

        if (selectedStage != null)
        {
            BackgroundMissionManager.Instance.StartMission(selectedStage, selectedCompanions);
            selectedCompanions.Clear();
            PopulateCompanionSelection();
            RefreshMissionList();
        }
    }

    public void RefreshMissionList()
    {
        foreach (Transform child in missionListContainer)
            Destroy(child.gameObject);

        foreach (var mission in BackgroundMissionManager.Instance.activeMissions)
        {
            GameObject entry = Instantiate(missionEntryPrefab, missionListContainer);

            entry.transform.Find("StageNameText").GetComponent<TextMeshProUGUI>().text = mission.stageID;
            entry.transform.Find("StepsText").GetComponent<TextMeshProUGUI>().text =
                $"{mission.GetStepsSinceStart()} / {mission.RequiredSteps} steps";

            Button claimButton = entry.transform.Find("ClaimButton").GetComponent<Button>();
            claimButton.interactable = mission.IsComplete;

            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(() =>
            {
                BackgroundMissionManager.Instance.ClaimMission(mission);
                RefreshMissionList();
            });
        }
    }
}
*/