using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ResearchSaveData
{
    public List<string> unlockedNodeIDs = new();
    public string currentResearchNodeID;
    public int currentProgress;
}


public class ResearchTreeManager : MonoBehaviour
{
    public static ResearchTreeManager Instance;

    public List<ResearchNode> unlockedNodes = new();
    public ResearchNode currentResearch;
    public int currentProgress;


    //DEBUGGING TOOLS
    [ContextMenu("Clear Research Save")]
    public void ClearResearchSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        Debug.Log("[Research] Save cleared.");
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadResearch();
    }


    void OnEnable()
    {
        PlayerData.OnStepsAdded += HandleStepsAdded;
    }

    void OnDisable()
    {
        PlayerData.OnStepsAdded -= HandleStepsAdded;
    }

    void OnApplicationQuit() => SaveResearch();

    void OnApplicationPause(bool pause)
    {
        if (pause) SaveResearch();
    }


    public bool CanResearch(ResearchNode node)
    {
        if (unlockedNodes.Contains(node)) return false;
        foreach (var pre in node.prerequisites)
            if (!unlockedNodes.Contains(pre)) return false;
        return true;
    }

    public void StartResearch(ResearchNode node)
    {
        // If it's already the current research, don't restart
        if (currentResearch == node)
        {
            Debug.Log($"[Research] '{node.displayName}' is already in progress.");
            return;
        }

        if (!CanResearch(node))
        {
            Debug.LogWarning($"[Research] Cannot start '{node.displayName}' due to missing prerequisites.");
            return;
        }

        currentResearch = node;
        currentProgress = 0;
        Debug.Log("Started research on: " + node.displayName);
    }


    private void HandleStepsAdded(int steps)
    {
        if (currentResearch == null) return;

        currentProgress += steps;
        Debug.Log("Progress: " + currentProgress + "/" + currentResearch.stepCost);

        if (currentProgress >= currentResearch.stepCost)
        {
            Debug.Log("Research complete! Tap to claim.");
            // Show UI for claiming
        }
    }

    public void ClaimResearch()
    {
        if (currentResearch == null || currentProgress < currentResearch.stepCost) return;

        unlockedNodes.Add(currentResearch);

        ApplyUnlock(currentResearch);
        SaveResearch(); 
        Debug.Log("Claimed research: " + currentResearch.displayName);

        currentResearch = null;
        currentProgress = 0;
    }

    private void ApplyUnlock(ResearchNode node)
    {
        if (node.globalStatType == GlobalStatType.None) return;

        foreach (var data in GameManager.Instance.allCharacterData)
        {
            switch (node.globalStatType)
            {
                case GlobalStatType.Attack:
                    data.attackPower += node.globalStatAmount;
                    break;
                case GlobalStatType.Health:
                    data.maxHealth += node.globalStatAmount;
                    break;
                case GlobalStatType.Defense:
                    data.defensePower += node.globalStatAmount;
                    break;
                case GlobalStatType.Speed:
                    data.speed += node.globalStatAmount;
                    break;
            }

            data.SaveData();
        }

    }



    private const string SaveKey = "ResearchSave";

public void SaveResearch()
{
    ResearchSaveData data = new ResearchSaveData
    {
        unlockedNodeIDs = unlockedNodes.Select(n => n.nodeID).ToList(),
        currentResearchNodeID = currentResearch != null ? currentResearch.nodeID : null,
        currentProgress = currentProgress
    };

    string json = JsonUtility.ToJson(data);
    PlayerPrefs.SetString(SaveKey, json);
    PlayerPrefs.Save();
    Debug.Log("[Research] Saved.");
}

public void LoadResearch()
{
    if (!PlayerPrefs.HasKey(SaveKey)) return;

    string json = PlayerPrefs.GetString(SaveKey);
    ResearchSaveData data = JsonUtility.FromJson<ResearchSaveData>(json);

    // Resolve ResearchNode references from Resources folder
    ResearchNode[] allNodes = Resources.LoadAll<ResearchNode>("ResearchNodes");
    unlockedNodes.Clear();

    foreach (string id in data.unlockedNodeIDs)
    {
        ResearchNode found = allNodes.FirstOrDefault(n => n.nodeID == id);
        if (found != null) unlockedNodes.Add(found);
    }

    if (!string.IsNullOrEmpty(data.currentResearchNodeID))
    {
        currentResearch = allNodes.FirstOrDefault(n => n.nodeID == data.currentResearchNodeID);
        currentProgress = data.currentProgress;
    }

    Debug.Log("[Research] Loaded.");
}



}
