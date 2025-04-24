using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character Data", order = 51)]
public class CharacterData : ScriptableObject
{
    // ────────────────────────────────────────────────────────────────
    #region Core fields (unchanged)

    public string  heroID;
    public int     heroLevel;
    public int     heroExp;
    public GameObject skillTreePanel;

    public Sprite  heroIcon;
    public Sprite  fullHeroImage;

    public int  heroStatPoints;
    public int  heroSkillPoints;
    public int  attackPower;
    public int  defensePower;
    public int  stamina;
    public int  maxStamina;
    public int  maxHealth;
    public int  health;
    public int  maxEnergy;
    public int  energy;
    public int  speed;
    public bool isUnlocked;
    public int  expToLevel;
    public float damageReflectionPercentage;

    public SkillType skillOne;
    public SkillType skillTwo;
    public SkillType skillThree;
    public SkillType skillFour;
    public SkillType skillFive;
    public SkillType skillSix;

    public List<SkillType> AvailableSkills;
    public List<SkillType> LockedSkills;

    #endregion
    //────────────────────────────────────────────────────────────────

    // ────────────────────────────────────────────────────────────────
    #region Equipment support (NEW)

    /// <summary>Serializable “slot → item” pair.</summary>
    [System.Serializable]
    public struct EquipSlot
    {
        public EquipmentType slot;
        public Equipment     item;   // reference to the Equipment ScriptableObject / prefab
    }

    // All currently-equipped items.
    [SerializeField] private List<EquipSlot> equipped = new();

    /// <summary>Returns the item in a slot, or null.</summary>
    public Equipment GetEquipped(EquipmentType slot)
    {
        var index = equipped.FindIndex(s => s.slot == slot);
        return index >= 0 ? equipped[index].item : null;
    }

    /// <summary>Equip an item and apply its stat bonuses. Replaces anything in the same slot.</summary>
    public void EquipItem(Equipment eq)
    {
        if (eq == null) { Debug.LogWarning("Tried to equip NULL."); return; }

        // Remove current item (if any)
        UnequipItem(eq.equipmentType);

        // Store new item
        equipped.RemoveAll(s => s.slot == eq.equipmentType);
        equipped.Add(new EquipSlot { slot = eq.equipmentType, item = eq });

        ApplyStatBonuses(eq);
    }

    /// <summary>Unequip whatever is in the slot.</summary>
    public void UnequipItem(EquipmentType slot)
    {
        var current = GetEquipped(slot);
        if (current != null) RemoveStatBonuses(current);

        equipped.RemoveAll(s => s.slot == slot);
    }

    /// <summary>Handy helper if UI needs a dictionary.</summary>
    public Dictionary<EquipmentType, Equipment> GetEquippedDictionary()
    {
        var dict = new Dictionary<EquipmentType, Equipment>();
        foreach (var es in equipped) dict[es.slot] = es.item;
        return dict;
    }

    private void ApplyStatBonuses(Equipment eq)
    {
        attackPower  += eq.attackBonus;
        defensePower += eq.defenseBonus;
        // add more stats here if your Equipment supplies them
    }

    private void RemoveStatBonuses(Equipment eq)
    {
        attackPower  -= eq.attackBonus;
        defensePower -= eq.defenseBonus;
    }

    #endregion
    //────────────────────────────────────────────────────────────────

    // ────────────────────────────────────────────────────────────────
    #region Skills (unchanged)
     public int baseExp = 30; // Starting value for experience points
    public float growthFactor = 1.1f; // Growth factor for exponential increase
    public int ExpToNextLevel(int heroLevel)
    {
        int exp = Mathf.RoundToInt(baseExp * Mathf.Pow(growthFactor, heroLevel));
         Debug.Log($"EXP to next level (Level {heroLevel}): {exp} current EXP: {heroExp}");
        return exp;
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
                skillInstance.ApplyPassiveEffect(this);

            Debug.Log(skillType + " unlocked.");
        }
        else { Debug.LogError(skillType + " is not in the LockedSkills list."); }
    }

    public Skill GetSkillInstance(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Slash               => new Slash(),
            SkillType.TripleHit           => new TripleHitSkill(),
            SkillType.Taunt               => new Taunt(),
            SkillType.ReflectDamagePassive=> new ReflectDamagePassive(),
            SkillType.SpeedBreak          => new SpeedBreak(),
            _ => null
        };
    }

    #endregion
    //────────────────────────────────────────────────────────────────

    // ────────────────────────────────────────────────────────────────
    #region Save / Load

    /// <remarks>
    ///  JsonUtility will serialise object references inside <see cref="EquipSlot"/>.
    ///  If you plan to load data across sessions or platforms, consider saving an
    ///  item ID (string) instead of the object reference, then look it up in an
    ///  ItemDatabase when loading.
    /// </remarks>
    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("CharacterData_" + heroID, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        Sprite savedHeroIcon      = heroIcon;
        Sprite savedFullHeroImage = fullHeroImage;

        string jsonData = PlayerPrefs.GetString("CharacterData_" + heroID, "{}");
        if (jsonData != "{}") JsonUtility.FromJsonOverwrite(jsonData, this);

        heroIcon       = savedHeroIcon;
        fullHeroImage  = savedFullHeroImage;
    }

    #endregion
    //────────────────────────────────────────────────────────────────
}
