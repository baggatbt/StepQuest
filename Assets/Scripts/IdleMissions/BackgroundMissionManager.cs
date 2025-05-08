using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMissionManager : MonoBehaviour
{
    public static BackgroundMissionManager Instance;
    public List<BackgroundBattleMission> activeMissions = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMissions();
        }
        else Destroy(gameObject);
    }

    public void StartMission(StageData stageData, List<Companion> companions)
{
    var mission = new BackgroundBattleMission
    {
        stageID = stageData.stageID,
        assignedHeroIDs = companions.Select(c => c.heroID).ToList(),
        stepsAtStart = PlayerData.Instance.inGameSteps
    };
    mission.InitializeTime();

    activeMissions.Add(mission);

    foreach (var c in companions)
        GameManager.Instance.currentParty.Remove(c);

    GameManager.Instance.SaveCurrentParty();
    SaveMissions();
}


    public void ClaimMission(BackgroundBattleMission mission)
    {
        int exp = mission.CalculateExpReward();
        int gold = mission.CalculateGoldReward();

        PlayerData.Instance.totalCopper += gold;

        foreach (string heroID in mission.assignedHeroIDs)
            GameManager.Instance.UpdateCompanionExp(heroID, exp);

        activeMissions.Remove(mission);
        SaveMissions();
    }

    public void SaveMissions()
    {
        string json = JsonUtility.ToJson(new Wrapper<BackgroundBattleMission>(activeMissions));
        PlayerPrefs.SetString("BackgroundMissions", json);
        PlayerPrefs.Save();
    }

    public void LoadMissions()
    {
        string json = PlayerPrefs.GetString("BackgroundMissions", "{}");
        var loaded = JsonUtility.FromJson<Wrapper<BackgroundBattleMission>>(json);
        if (loaded != null && loaded.Items != null)
        {
            activeMissions = loaded.Items;
            foreach (var m in activeMissions)
                m.RestoreTime();
        }
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> Items = new();
        public Wrapper() { }
        public Wrapper(List<T> items) => Items = items;
    }
}
