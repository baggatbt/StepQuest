using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Knight : Character
{
    // Any specific properties or methods unique to the Knight.
    // For example:
    public int shieldPower;
    
    protected override void Awake()
    {
        base.Awake();
        level = 1; 
        attackPower = 2;
        maxHealth = 10;
        health = maxHealth;
        maxEnergy = 5;
        energy = maxEnergy;
        defensePower = 0;
        speed = 2;
        atkGrowth = 3;
        defGrowth = 1;
    }

    

    // ... Any additional methods or behaviors for the Knight.
}
