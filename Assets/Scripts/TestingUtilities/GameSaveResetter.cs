using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveResetter : MonoBehaviour
{
    [Serializable]
    public class CharacterResetPair
    {
        public CharacterData liveData;
        public CharacterData defaultData;
    }

    [Header("Character Data Reset")]
    [SerializeField] private List<CharacterResetPair> characterDataToReset = new List<CharacterResetPair>();

    [Header("Options")]
    [SerializeField] private bool reloadSceneAfterReset = true;

    public void ResetAllSaveData()
    {
        Debug.Log("[GameSaveResetter] Resetting all save data...");

        // 1. Stop old active companions from saving stale data back.
        DestroyActiveCompanionObjects();

        // 2. Clear PlayerPrefs.
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 3. Delete inventory JSON.
        DeleteInventorySaveFile();

        // 4. Reset CharacterData ScriptableObjects from their default copies.
        ResetCharacterDataObjects();

        // 5. Reset runtime singleton state.
        ResetRuntimeObjects();

        Debug.Log("[GameSaveResetter] Reset complete.");

        if (reloadSceneAfterReset)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void ResetCharacterDataObjects()
    {
        foreach (CharacterResetPair pair in characterDataToReset)
        {
            if (pair.liveData == null || pair.defaultData == null)
            {
                Debug.LogWarning("[GameSaveResetter] Missing liveData or defaultData.");
                continue;
            }

            Debug.Log(
                $"[GameSaveResetter] Before reset: {pair.liveData.heroID} " +
                $"Lv {pair.liveData.heroLevel}, EXP {pair.liveData.heroExp}, " +
                $"HP {pair.liveData.health}/{pair.liveData.maxHealth}, " +
                $"ATK {pair.liveData.attackPower}"
            );

            if (pair.liveData.heroID == "Knight")
{
    pair.liveData.ResetKnightToDefaults();
}
else
{
    pair.liveData.ResetFromDefault(pair.defaultData);
}

            Debug.Log(
                $"[GameSaveResetter] After reset: {pair.liveData.heroID} " +
                $"Lv {pair.liveData.heroLevel}, EXP {pair.liveData.heroExp}, " +
                $"HP {pair.liveData.health}/{pair.liveData.maxHealth}, " +
                $"ATK {pair.liveData.attackPower}"
            );
        }

        PlayerPrefs.Save();
    }

    private void DestroyActiveCompanionObjects()
    {
        Companion[] activeCompanions = FindObjectsOfType<Companion>(true);

        foreach (Companion companion in activeCompanions)
        {
            if (companion != null)
            {
                Debug.Log("[GameSaveResetter] Destroying active companion instance: " + companion.name);
                Destroy(companion.gameObject);
            }
        }
    }

    private void DeleteInventorySaveFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "inventory.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("[GameSaveResetter] Deleted inventory save file: " + filePath);
        }
        else
        {
            Debug.Log("[GameSaveResetter] No inventory save file found.");
        }
    }

    private void ResetRuntimeObjects()
    {
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.level = 1;
            PlayerData.Instance.exp = 0;
            PlayerData.Instance.totalCopper = 0;
            PlayerData.Instance.inGameSteps = 0;
            PlayerData.Instance.baselineSteps = 0;
            PlayerData.Instance.currentSensorTotal = 0;
            PlayerData.Instance.currentStageIndex = 0;
            PlayerData.Instance.firstTimeLogin = true;

            PlayerData.Instance.SavePlayerData();

            Debug.Log("[GameSaveResetter] Runtime PlayerData reset.");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.itemList.Clear();
            GameManager.Instance.currentParty.Clear();
            GameManager.Instance.companions.Clear();
            GameManager.Instance.UnlockedStageNames.Clear();
            GameManager.Instance.UnlockedStageNames.Add("0");

            // Rebuild dictionary from reset data.
            GameManager.Instance.characterDataDictionary.Clear();

            foreach (CharacterResetPair pair in characterDataToReset)
            {
                if (pair.liveData == null) continue;

                pair.liveData.LoadData();

                if (!GameManager.Instance.characterDataDictionary.ContainsKey(pair.liveData.heroID))
                {
                    GameManager.Instance.characterDataDictionary.Add(pair.liveData.heroID, pair.liveData);
                }
                else
                {
                    GameManager.Instance.characterDataDictionary[pair.liveData.heroID] = pair.liveData;
                }
            }

            Debug.Log("[GameSaveResetter] Runtime GameManager reset.");
        }
    }
}