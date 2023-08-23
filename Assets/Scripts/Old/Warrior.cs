using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Character
{
    // Constructor
    public Warrior()
    {
        // Assign the skill to this specific type of character
        this.currentSkill = new TripleHitSkill(); 
    }

    // Other methods and properties specific to warrior
}
