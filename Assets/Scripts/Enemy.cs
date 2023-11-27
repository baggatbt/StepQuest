using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int expReward;
    public int goldReward;
    public BattlePosition PreferredAttackPosition { get; protected set; }
    
   

    // Additional enemy-specific properties and behavior
}

public enum BattlePosition
{
    Front,
    Back,
}
