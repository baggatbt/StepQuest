using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public PlayerAccountData playerAccountData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadPlayerAccountData();
    }

    public void SavePlayerAccountData()
    {
        string json = JsonUtility.ToJson(playerAccountData);
        PlayerPrefs.SetString("PlayerAccountData", json);
        PlayerPrefs.Save();
    }

    public void LoadPlayerAccountData()
    {
        if (PlayerPrefs.HasKey("PlayerAccountData"))
        {
            string json = PlayerPrefs.GetString("PlayerAccountData");
            playerAccountData = JsonUtility.FromJson<PlayerAccountData>(json);
        }
        else
        {
            // Initialize with default values if no saved data exists
            playerAccountData = new PlayerAccountData
            {
                level = 1,
                totalCopper = 0,
                currentSteps = 0,
                inGameSteps = 0
                // Initialize other fields as necessary
            };
        }
    }

    public void IncrementInGameSteps()
    {
        playerAccountData.inGameSteps++;
        SavePlayerAccountData();
        Debug.Log("In-game steps: " + playerAccountData.inGameSteps);
    }

    private void OnApplicationQuit()
    {
        SavePlayerAccountData();
    }
}
