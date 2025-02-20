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
    
    // These fields are serialized so you can set them in the Editor.
    // Their values will be preserved during load.
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
    public float damageReflectionPercentage;
    public SkillType skillOne;
    public SkillType skillTwo;
    public SkillType skillThree;
    public SkillType skillFour;
    public SkillType skillFive;
    public SkillType skillSix;
    public List<SkillType> AvailableSkills;
    public List<SkillType> LockedSkills;

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
            
            // Find the skill instance and apply its passive effect if applicable
            Skill skillInstance = GetSkillInstance(skillType);
            if (skillInstance != null && !skillInstance.isActiveSkill)
            {
                skillInstance.ApplyPassiveEffect(this);
            }
            
            Debug.Log(skillType.ToString() + " unlocked.");
        }
        else
        {
            Debug.LogError(skillType.ToString() + " is not in the LockedSkills list.");
        }
    }

    public Skill GetSkillInstance(SkillType skillType)
    {
        // This method returns a new instance of the specified skill.
        // (Assumes that your Skill, Slash, TripleHitSkill, etc. classes are defined elsewhere.)
        switch (skillType)
        {
            case SkillType.Slash:
                return new Slash();
            case SkillType.TripleHit:
                return new TripleHitSkill();
            case SkillType.Taunt:
                return new Taunt();
            case SkillType.ReflectDamagePassive:
                return new ReflectDamagePassive();
            case SkillType.SpeedBreak:
                return new SpeedBreak();
            default:
                Debug.LogError("Unknown skill type: " + skillType);
                return null;
        }
    }

    public void SaveData()
    {
        // Convert the CharacterData to JSON and save it in PlayerPrefs.
        string jsonData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("CharacterData_" + heroID, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        // Save the current sprite references before loading JSON.
        Sprite savedHeroIcon = heroIcon;
        Sprite savedFullHeroImage = fullHeroImage;

        string jsonData = PlayerPrefs.GetString("CharacterData_" + heroID, "{}");
        if (jsonData != "{}")
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }

        // Restore the sprite references so they remain unchanged.
        heroIcon = savedHeroIcon;
        fullHeroImage = savedFullHeroImage;
    }
}
