using UnityEngine;
using System.Collections.Generic;

public class TaskSkillManager : MonoBehaviour
{
    public static TaskSkillManager Instance { get; private set; }

    [System.Serializable]
    public class SkillEntry
    {
        public string skillID;
        [Min(1)] public int level = 1;
        [Min(0)] public int xp    = 0;
    }

    [Header("For testing: override these on Awake")]
    public List<SkillEntry> initialSkills = new List<SkillEntry>();

    [Header("Formula-based XP tables")]
    public SkillConfig[]    skillConfigs;

    Dictionary<string, SkillConfig> configMap;
    Dictionary<string,int>          levels;
    Dictionary<string,int>          xps;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            configMap = new Dictionary<string, SkillConfig>();
            foreach (var cfg in skillConfigs)
                if (!string.IsNullOrEmpty(cfg.skillID))
                    configMap[cfg.skillID] = cfg;

            foreach (var entry in initialSkills)
            {
                PlayerPrefs.SetInt($"Skill_{entry.skillID}_Level", entry.level);
                PlayerPrefs.SetInt($"Skill_{entry.skillID}_XP",    entry.xp);
            }
            PlayerPrefs.Save();

            levels = new Dictionary<string,int>();
            xps     = new Dictionary<string,int>();
        }
        else Destroy(gameObject);
    }

    /// <summary>Get saved level (default 1).</summary>
    public int GetLevel(string skillID)
    {
        if (!levels.ContainsKey(skillID))
            levels[skillID] = PlayerPrefs.GetInt($"Skill_{skillID}_Level", 1);
        return levels[skillID];
    }

    /// <summary>Get current XP progress (default 0).</summary>
    public int GetCurrentXP(string skillID)
    {
        if (!xps.ContainsKey(skillID))
            xps[skillID] = PlayerPrefs.GetInt($"Skill_{skillID}_XP", 0);
        return xps[skillID];
    }

    /// <summary>XP needed to go from level → level+1.</summary>
    public int GetXPThreshold(string skillID, int level)
    {
        if (configMap.TryGetValue(skillID, out var cfg))
        {
            if (level >= cfg.maxLevel)
                return int.MaxValue;
            // exponential growth: baseXP * growthRate^(level-1) + additive
            float xp = cfg.baseXP * Mathf.Pow(cfg.growthRate, level - 1) + cfg.additive;
            return Mathf.Max(1, Mathf.RoundToInt(xp));
        }
        return level * 100;
    }

    /// <summary>How much XP you need right now to reach next level.</summary>
    public int GetXPToNextLevel(string skillID)
    {
        int lvl = GetLevel(skillID);
        return GetXPThreshold(skillID, lvl);
    }

    /// <summary>How much XP remains (threshold − current).</summary>
    public int GetXPRemaining(string skillID)
    {
        return GetXPToNextLevel(skillID) - GetCurrentXP(skillID);
    }

    /// <summary>0–1 progress toward next level.</summary>
    public float GetProgress01(string skillID)
    {
        int current = GetCurrentXP(skillID);
        int needed  = GetXPToNextLevel(skillID);
        return Mathf.Clamp01((float)current / needed);
    }


    public void AddXP(string skillID, int gain)
    {
        if (!xps.ContainsKey(skillID))
            xps[skillID] = PlayerPrefs.GetInt($"Skill_{skillID}_XP", 0);

        xps[skillID] += gain;
        int lvl    = GetLevel(skillID);
        int needed = GetXPThreshold(skillID, lvl);

        // level up while we have enough XP
        while (xps[skillID] >= needed)
        {
            xps[skillID] -= needed;
            lvl++;
            levels[skillID] = lvl;
            PlayerPrefs.SetInt($"Skill_{skillID}_Level", lvl);
            needed = GetXPThreshold(skillID, lvl);
        }

        PlayerPrefs.SetInt($"Skill_{skillID}_XP", xps[skillID]);
        PlayerPrefs.Save();
    }
}
