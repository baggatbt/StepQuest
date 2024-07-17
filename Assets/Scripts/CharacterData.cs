using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character Data", order = 51)]
public class CharacterData : ScriptableObject
{
    public string heroID;
    public int heroLevel;
    public int heroExp;
    public GameObject skillTreePanel;
    public Sprite heroIcon;
    public Sprite fullHeroImage;
    public int heroStatPoints;
    public int heroSkillPoints;
    public int attackPower;
    public int defensePower;
    public int stamina;
    public int maxStamina;
    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public int speed;
    public bool isUnlocked;
    public int expToLevel;

    public int ExpToNextLevel(int heroLevel)
    {
        Debug.Log("EXP to level : " + (30 * heroLevel * heroLevel));
        return 30 * heroLevel * heroLevel;
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("CharacterData_" + heroID, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        string jsonData = PlayerPrefs.GetString("CharacterData_" + heroID, "{}");
        if (jsonData != "{}")
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }
    }

    public void InitializeDefaults()
    {
        attackPower = 5;
        defensePower = 2;
        maxHealth = 12;
        health = maxHealth;
        speed = 4;
        maxEnergy = 5;
        energy = maxEnergy;
        heroID = "Knight";
        heroLevel = 1;
        heroExp = 0;
        heroSkillPoints = 0;
        heroStatPoints = 0;
        maxStamina = 10;
        stamina = maxStamina;
        expToLevel = ExpToNextLevel(heroLevel);
        
    }
}
