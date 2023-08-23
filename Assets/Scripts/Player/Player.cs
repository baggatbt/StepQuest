using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : Character
{
    public Transform playerSpawnPoint;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI defensePowerText;
    public TextMeshProUGUI stepsText;

    private PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();
        playerData = PlayerData.Instance;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerInitialized"))
        {
            LoadFromPlayerData();
            Debug.Log("Already init'd, this is loading");
        }
        else
        {
            InitializePlayer();
            Debug.Log("making new player");
        }

        UpdateUI();  // Update the UI with initial values.
    }

    private void InitializePlayer()
    {
        if (!PlayerPrefs.HasKey("PlayerInitialized"))
        {
            playerData.level = 1;
            playerData.exp = 0;
            playerData.gold = 100;
            playerData.attackPower = 10;
            playerData.defensePower = 5;
            playerData.inGameSteps = 0;

            PlayerPrefs.SetInt("PlayerInitialized", 1);
            PlayerPrefs.Save();

            // Save the initialized data
            playerData.SavePlayerData();
        }
    }

    private void LoadFromPlayerData()
    {
        playerData.LoadPlayerData();
    }

    private void Update()
    {
        // Update UI regularly or as per your needs
        UpdateUI();
    }

    private void UpdateUI()
{
    if (levelText != null) levelText.text = "Level: " + playerData.level.ToString();
    if (expText != null) expText.text = "Exp: " + playerData.exp.ToString();
    if (goldText != null) goldText.text = "Gold: " + playerData.gold.ToString();
    if (attackPowerText != null) attackPowerText.text = "Attack Power: " + playerData.attackPower.ToString();
    if (defensePowerText != null) defensePowerText.text = "Defense Power: " + playerData.defensePower.ToString();
    if (stepsText != null) stepsText.text = "Steps: " + playerData.inGameSteps.ToString();
}


     // Now, when I  need to update the player's attributes, just update them in the `playerData` 
    // and then call the `SavePlayerData()` method of the `playerData`.
    public void RewardGold(int amount)
    {
        playerData.gold += amount;
        playerData.SavePlayerData();
    }

}









