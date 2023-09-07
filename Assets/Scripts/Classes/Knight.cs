using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Knight : Character
{
    // Any specific properties or methods unique to the Knight.
    
    public string jobClass = "Adventurer";

    // Static growth rates for the Adventurer class
    public static int atkGrowth = 2;
    public static int defGrowth = 3;
    public static int magAtkGrowth = 1;
    public static int magDefGrowth = 1;
    //public int shieldPower;
    
    protected override void Awake()
    {
        base.Awake();
        
       
    }

    

    // ... Any additional methods or behaviors for the Knight.
}
