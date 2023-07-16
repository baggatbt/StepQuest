using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public string playerClass; // Player's class e.g. "Warrior", "Mage", etc.
    public int level; // Player's level
    public int exp; // Player's experience points
    public int gold; // Player's gold

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // Call the Start function of the base class (Character)

        // Initialize player specific stats
        level = 1;
        exp = 0;
        gold = 0;
    }

    // Method to add experience points to the player and level up if necessary
    public void GainExp(int amount)
    {
        exp += amount;

        // Simple level up logic: level up every 100 exp points
        while (exp >= 100)
        {
            exp -= 100;
            level++;
            Debug.Log("Level up! Now at level " + level);
        }
    }

    // Method to add gold to the player
    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log("Gained " + amount + " gold. Now have " + gold + " gold");
    }

    // Method to deduct gold from the player
    public void SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log("Spent " + amount + " gold. Now have " + gold + " gold");
        }
        else
        {
            Debug.Log("Not enough gold. Need " + (amount - gold) + " more gold");
        }
    }
}
