using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Character
{
    // Constructor
    public Slime()
    {
        // Assign the skill to this specific type of character
        this.currentSkill = new SimpleAttackSkill(); // TODO: Change this to a slime skill
    }

    // Other methods and properties specific to Slime...
}
