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
    public Sprite heroIcon; // Set this in the Editor
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
    public SkillType skillOne;
    public SkillType skillTwo;
    public SkillType skillThree;
    public SkillType skillFour;
    public SkillType skillFive;
    public SkillType skillSix;
    public  List<SkillType> AvailableSkills;
    public  List<SkillType> LockedSkills;

    // Do not touch this field after setting it in the Editor
    // Remove methods that might modify heroIcon like OnEnable or LoadSprite
    
    public int ExpToNextLevel(int heroLevel)
    {
        Debug.Log("EXP to level : " + (30 * heroLevel * heroLevel));
        return 30 * heroLevel * heroLevel;
    }

    public void UnlockSkill(SkillType skillType)
    {
        if (LockedSkills.Contains(skillType))
        {
            LockedSkills.Remove(skillType);
            AvailableSkills.Add(skillType);
            Debug.Log(skillType.ToString() + " unlocked.");
        }
        else
        {
            Debug.LogError(skillType.ToString() + " is not in the LockedSkills list.");
        }
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
}
