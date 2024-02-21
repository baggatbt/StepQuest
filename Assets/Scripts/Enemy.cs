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

    // Additional enemy-specific properties and behavior

    protected override void Awake()
    {
        base.Awake();
        UpdateExpAndGoldRewards();
    }

    protected virtual void UpdateExpAndGoldRewards()
    {
        this.expReward = CalculateExpReward(this.level);
        this.goldReward = CalculateGoldReward(this.level);
    }

    protected int CalculateExpReward(int level)
    {
        if (level < 20)
        {
            // For levels 1-20, use a quadratic polynomial formula
            return 100 * level * level;
        }
        else
        {
            // Beyond level 20, use an exponential model to steeply increase EXP requirements
            return (int)(100 * Mathf.Pow(1.5f, level - 19) * 400); // 400 is 20^2, the base EXP at level 20
        }
    }

    protected int CalculateGoldReward(int level)
    {
        // Example gold reward calculation. 
        return 3 + (level * 2);
    }
}
