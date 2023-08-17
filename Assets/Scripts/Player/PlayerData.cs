using System;
using UnityEngine;
using System.Collections.Generic;  

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public int level;
    public int exp;
    public int gold;
    public int attackPower;
    public int defensePower;
    public int steps;
    public int speed;
    public Dictionary<string, int> skillLevels; // Keep track of each skill's level
    public Dictionary<string, int> skillExp; // Keep track of each skill's experience
    public bool isInitialized = false;


    private StepCounterController stepCounterController;

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
    }

    private void Start()
    {
        stepCounterController = FindObjectOfType<StepCounterController>();
        
        if (stepCounterController != null)
        {
            steps = stepCounterController.GetStepsSinceStart();
            Debug.Log(steps);
        }
    }

    private void Update()
    {
        steps = stepCounterController.GetStepsSinceStart();
    }

    // Initialization method to setup data
    public void Initialize(Player player)
    {
        if (isInitialized) return; // Skip if already initialized
        
        level = player.level;
        exp = player.exp;
        gold = player.gold;
        attackPower = player.attackPower;
        defensePower = player.defensePower;
        steps = player.steps;
        speed = player.speed;
        // Initialize the skillLevels dictionary
        skillLevels = new Dictionary<string, int>();
        skillExp = new Dictionary<string, int>();

        isInitialized = true; // Mark as initialized
    }

    // Method to increase the level of a skill
    public void IncreaseSkillLevel(string skillName, int amount)
    {
        if (skillLevels.ContainsKey(skillName))
        {
            skillLevels[skillName] += amount; // Usage: PlayerData.Instance.IncreaseSkillLevel("Slash", 1);  // Increase the level of Slash skill by 1

        }
        else
        {
            skillLevels[skillName] = amount;
        }
    }

     // Method to increase the experience of a skill
    public void IncreaseSkillExp(string skillName, int amount)
    {
        if (skillExp.ContainsKey(skillName))
        {
            skillExp[skillName] += amount;
        }
        else
        {
            skillExp[skillName] = amount;
        }
    }



    
}


