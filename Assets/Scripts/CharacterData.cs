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
    public int stamina;
    public int maxStamina;
    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public int speed;
    public bool isUnlocked;

    public int ExpToNextLevel(int heroLevel)
    {
        Debug.Log("EXP to level : " + (30 * heroLevel * heroLevel));
        return 30 * heroLevel * heroLevel;
    }
}

