using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseUp : StatusEffect
{
    public int defenseModifier;

    public DefenseUp(float effectDuration)
    {
        effectName = "Defense Up";
        
    }

    public override void ApplyEffect(Character target)
    {
        defenseModifier = (target.defensePower / 2); //Half their defense
        target.defensePower += defenseModifier;
    }

    public override void RemoveEffect(Character target)
    {
        target.defensePower -= defenseModifier;
    }

  

}
