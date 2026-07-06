using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character Data", order = 51)]
public class CharacterData : ScriptableObject
{
    // ────────────────────────────────────────────────────────────────
    #region Core fields

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
    public float damageReflectionPercentage;

    public SkillType skillOne;
    public SkillType skillTwo;
    public SkillType skillThree;
    public SkillType skillFour;
    public SkillType skillFive;
    public SkillType skillSix;

    public List<SkillType> AvailableSkills;
    public List<SkillType> LockedSkills;
    public List<SkillType> equippedSkills = new List<SkillType>();
    public int maxEquippedSkills = 3;

    [Header("Knight Progression")]
    public KnightSlashModifier knightSlashModifier = KnightSlashModifier.None;
    public KnightPassive knightPassive = KnightPassive.None;
    public KnightPath knightPath = KnightPath.None;

    public bool hasChosenLevel6Modifier;
    public bool hasChosenLevel8Passive;
    public bool hasChosenLevel10Path;

    public ClassMastery classMastery = new ClassMastery();

    #endregion

    // ────────────────────────────────────────────────────────────────
    #region Equipment support

    [System.Serializable]
    public struct EquipSlot
    {
        public EquipmentType slot;
        public Equipment item;
    }

    [System.Serializable]
    public class EquippedEquipmentSaveData
    {
        public bool hasItem;

        public EquipmentType slot;
        public int itemID;
        public string uniqueInstanceId;

        public int attackBonus;
        public int defenseBonus;
        public int maxHealthBonus;
        public int maxEnergyBonus;
        public int speedBonus;
    }

    [Header("Runtime Equipped Items")]
    [SerializeField] private List<EquipSlot> equipped = new List<EquipSlot>();

    [Header("Saved Equipped Items")]
    [SerializeField] private List<EquippedEquipmentSaveData> savedEquippedItems = new List<EquippedEquipmentSaveData>();

    public Equipment GetEquipped(EquipmentType slot)
    {
        EnsureEquipmentLists();

        int index = equipped.FindIndex(s => s.slot == slot);
        return index >= 0 ? equipped[index].item : null;
    }

    public void EquipItem(Equipment eq)
    {
        if (eq == null)
        {
            Debug.LogWarning("[CharacterData] Tried to equip NULL.");
            return;
        }

        EnsureEquipmentLists();

        // Remove current item from this slot first.
        UnequipItem(eq.equipmentType);

        // Store new item.
        equipped.RemoveAll(s => s.slot == eq.equipmentType);
        equipped.Add(new EquipSlot
        {
            slot = eq.equipmentType,
            item = eq
        });

        ApplyStatBonuses(eq);
        SaveEquippedGearToSaveFields();
    }

    public void UnequipItem(EquipmentType slot)
    {
        EnsureEquipmentLists();

        Equipment current = GetEquipped(slot);

        if (current != null)
            RemoveStatBonuses(current);

        equipped.RemoveAll(s => s.slot == slot);
        SaveEquippedGearToSaveFields();
    }

    public Dictionary<EquipmentType, Equipment> GetEquippedDictionary()
    {
        EnsureEquipmentLists();

        Dictionary<EquipmentType, Equipment> dict = new Dictionary<EquipmentType, Equipment>();

        foreach (EquipSlot es in equipped)
        {
            if (es.item == null)
                continue;

            dict[es.slot] = es.item;
        }

        return dict;
    }

    private void ApplyStatBonuses(Equipment eq)
    {
        if (eq == null)
            return;

        attackPower += eq.attackBonus;
        defensePower += eq.defenseBonus;

        maxHealth += eq.maxHealthBonus;
        health += eq.maxHealthBonus;

        maxEnergy += eq.maxEnergyBonus;
        energy += eq.maxEnergyBonus;

        speed += eq.speedBonus;
    }

    private void RemoveStatBonuses(Equipment eq)
    {
        if (eq == null)
            return;

        attackPower -= eq.attackBonus;
        defensePower -= eq.defenseBonus;

        maxHealth -= eq.maxHealthBonus;
        health = Mathf.Clamp(health - eq.maxHealthBonus, 1, Mathf.Max(1, maxHealth));

        maxEnergy -= eq.maxEnergyBonus;
        energy = Mathf.Clamp(energy - eq.maxEnergyBonus, 0, Mathf.Max(0, maxEnergy));

        speed -= eq.speedBonus;
    }

    private void EnsureEquipmentLists()
    {
        if (equipped == null)
            equipped = new List<EquipSlot>();

        if (savedEquippedItems == null)
            savedEquippedItems = new List<EquippedEquipmentSaveData>();
    }

    private void SaveEquippedGearToSaveFields()
    {
        EnsureEquipmentLists();

        savedEquippedItems.Clear();

        foreach (EquipSlot slot in equipped)
        {
            if (slot.item == null)
                continue;

            Equipment eq = slot.item;

            EquippedEquipmentSaveData data = new EquippedEquipmentSaveData
            {
                hasItem = true,
                slot = slot.slot,
                itemID = eq.itemID,
                uniqueInstanceId = eq.uniqueInstanceId,

                attackBonus = eq.attackBonus,
                defenseBonus = eq.defenseBonus,
                maxHealthBonus = eq.maxHealthBonus,
                maxEnergyBonus = eq.maxEnergyBonus,
                speedBonus = eq.speedBonus
            };

            savedEquippedItems.Add(data);
        }
    }

    private void LoadEquippedGearFromSaveFields()
    {
        EnsureEquipmentLists();

        equipped.Clear();

        if (savedEquippedItems == null || savedEquippedItems.Count == 0)
            return;

        foreach (EquippedEquipmentSaveData data in savedEquippedItems)
        {
            Equipment loadedEquipment = LoadEquipmentFromSaveData(data);

            if (loadedEquipment == null)
                continue;

            equipped.Add(new EquipSlot
            {
                slot = data.slot,
                item = loadedEquipment
            });
        }

        Debug.Log($"[CharacterData] Loaded equipped gear for {heroID}. Count: {equipped.Count}");
    }

    private Equipment LoadEquipmentFromSaveData(EquippedEquipmentSaveData data)
    {
        if (data == null || !data.hasItem)
            return null;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[CharacterData] Cannot load equipped gear because GameManager.Instance is null.");
            return null;
        }

        Item template = GameManager.Instance.FindItemInMasterList(data.itemID);

        if (template == null)
        {
            Debug.LogWarning("[CharacterData] Could not find equipped item template with ID: " + data.itemID);
            return null;
        }

        Equipment equipmentTemplate = template as Equipment;

        if (equipmentTemplate == null)
        {
            Debug.LogWarning("[CharacterData] Saved equipped item is not Equipment. Item ID: " + data.itemID);
            return null;
        }

        Equipment loadedEquipment = Instantiate(equipmentTemplate);

        loadedEquipment.LoadRolledData(
            data.uniqueInstanceId,
            data.attackBonus,
            data.defenseBonus,
            data.maxHealthBonus,
            data.maxEnergyBonus,
            data.speedBonus
        );

        loadedEquipment.quantity = 1;

        return loadedEquipment;
    }

    public void ClearEquippedGear()
    {
        EnsureEquipmentLists();

        equipped.Clear();
        savedEquippedItems.Clear();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────
    #region Skills

    public int baseExp = 30;
    public float growthFactor = 1.1f;

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

            Skill skillInstance = GetSkillInstance(skillType);

            if (skillInstance != null && !skillInstance.isActiveSkill)
                skillInstance.ApplyPassiveEffect(this);

            Debug.Log(skillType + " unlocked.");
        }
        else
        {
            Debug.LogError(skillType + " is not in the LockedSkills list.");
        }
    }

    public Skill GetSkillInstance(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Slash => new Slash(),
            SkillType.TripleHit => new TripleHitSkill(),
            SkillType.Taunt => new Taunt(),
            SkillType.ReflectDamagePassive => new ReflectDamagePassive(),
            SkillType.SpeedBreak => new SpeedBreak(),
            _ => null
        };
    }

    #endregion

    // ────────────────────────────────────────────────────────────────
    #region Save / Load / Reset

    public void SaveData()
    {
        SaveEquippedGearToSaveFields();

        string jsonData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("CharacterData_" + heroID, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        Sprite savedHeroIcon = heroIcon;
        Sprite savedFullHeroImage = fullHeroImage;
        GameObject savedSkillTreePanel = skillTreePanel;

        string jsonData = PlayerPrefs.GetString("CharacterData_" + heroID, null);

        if (!string.IsNullOrEmpty(jsonData))
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }

        // Restore references you do not want saved JSON to break.
        heroIcon = savedHeroIcon;
        fullHeroImage = savedFullHeroImage;
        skillTreePanel = savedSkillTreePanel;

        EnsureValidLevel();
        EnsureListsAfterLoad();

        // Important:
        // Rebuild equipped runtime ScriptableObject instances from saved plain data.
        // Do not apply stat bonuses here because the saved CharacterData stats already include them.
        LoadEquippedGearFromSaveFields();
    }

    private void EnsureListsAfterLoad()
    {
        if (AvailableSkills == null)
            AvailableSkills = new List<SkillType>();

        if (LockedSkills == null)
            LockedSkills = new List<SkillType>();

        if (equippedSkills == null)
            equippedSkills = new List<SkillType>();

        if (classMastery == null)
            classMastery = new ClassMastery();

        EnsureEquipmentLists();
    }

    public void ResetFromDefault(CharacterData defaultData)
    {
        if (defaultData == null)
        {
            Debug.LogError($"No default CharacterData provided for {heroID}.");
            return;
        }

        string oldHeroID = heroID;

        Sprite savedHeroIcon = heroIcon;
        Sprite savedFullHeroImage = fullHeroImage;
        GameObject savedSkillTreePanel = skillTreePanel;

        PlayerPrefs.DeleteKey("CharacterData_" + oldHeroID);

        string defaultJson = JsonUtility.ToJson(defaultData);
        JsonUtility.FromJsonOverwrite(defaultJson, this);

        heroIcon = savedHeroIcon;
        fullHeroImage = savedFullHeroImage;
        skillTreePanel = savedSkillTreePanel;

        EnsureListsAfterLoad();
        ClearEquippedGear();
        EnsureValidLevel();

        PlayerPrefs.DeleteKey("CharacterData_" + heroID);

        SaveData();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Reset CharacterData from default: {oldHeroID} -> {heroID}");
    }

    public void EnsureValidLevel()
    {
        if (heroLevel < 1)
        {
            heroLevel = 1;
            heroExp = 0;
            heroSkillPoints = 0;
            heroStatPoints = 0;
        }
    }

    public void ResetKnightToDefaults()
    {
        heroID = "Knight";

        heroLevel = 1;
        heroExp = 0;
        heroStatPoints = 0;
        heroSkillPoints = 0;

        attackPower = 2;
        defensePower = 1;

        maxHealth = 12;
        health = 12;

        maxEnergy = 10;
        energy = 10;

        maxStamina = 10;
        stamina = 10;

        speed = 4;

        isUnlocked = true;
        expToLevel = 30;
        damageReflectionPercentage = 0f;

        skillOne = SkillType.TripleHit;
        skillTwo = SkillType.Taunt;
        skillThree = SkillType.None;
        skillFour = SkillType.None;
        skillFive = SkillType.None;
        skillSix = SkillType.None;

        AvailableSkills = new List<SkillType>
        {
            SkillType.Slash
        };

        LockedSkills = new List<SkillType>
        {
            SkillType.TripleHit,
            SkillType.Taunt,
            SkillType.ReflectDamagePassive,
            SkillType.SpeedBreak
        };

        equippedSkills = new List<SkillType>();

        knightSlashModifier = KnightSlashModifier.None;
        knightPassive = KnightPassive.None;
        knightPath = KnightPath.None;

        hasChosenLevel6Modifier = false;
        hasChosenLevel8Passive = false;
        hasChosenLevel10Path = false;

        classMastery = new ClassMastery();

        ClearEquippedGear();

        PlayerPrefs.DeleteKey("CharacterData_" + heroID);
        SaveData();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log("[CharacterData] Knight reset to hardcoded defaults.");
    }

    #endregion
}