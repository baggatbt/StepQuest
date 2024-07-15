using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int expReward;
    public int goldReward;

    public int attackGrowth;
    public int defenseGrowth;
    public int healthGrowth;
    public Sprite enemyIcon;
    public MaterialItem[] possibleDrops; // Assign this in the Inspector with your material items.
    public int dropChancePercentage = 50; // Example drop chance.

    protected override void Awake()
    {
        base.Awake();
        UpdateExpAndGoldRewards();
    }

    protected virtual void UpdateExpAndGoldRewards()
    {
        expReward = CalculateExpReward(level);
        goldReward = CalculateGoldReward(level);
    }

    protected int CalculateExpReward(int level)
    {
        if (level < 20)
        {
            return 100 * level * level; // For levels 1-20, use a quadratic polynomial formula
        }
        else
        {
            return (int)(100 * Mathf.Pow(1.5f, level - 19) * 400); // Beyond level 20, exponential model
        }
    }

    protected int CalculateGoldReward(int level)
    {
        return 3 + (level * 2); // Example gold reward calculation. 
    }

    // Call this method when the enemy is defeated.
    public void DropMaterial()
    {
        if (Random.Range(0, 100) < dropChancePercentage)
        {
            int dropIndex = Random.Range(0, possibleDrops.Length);
            MaterialItem droppedMaterial = possibleDrops[dropIndex];

            Debug.Log("Dropped material: " + droppedMaterial.itemName);
            // Add the dropped material to the player's inventory via GameManager
            GameManager.Instance.AddItem(droppedMaterial);
        }
    }
}
