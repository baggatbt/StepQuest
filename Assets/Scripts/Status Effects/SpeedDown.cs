using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDown : StatusEffect
{
    public int speedModifier;
    
    public SpeedDown(float effectDuration)
    {
        effectName = "Speed Down";
        iconImage = LoadIconImage("SkillIcons/weapon1");
        
    }

    public override void ApplyEffect(Character target)
    {
        speedModifier = (target.speed / 2); 
        target.speed -= speedModifier;
        
    }

    public override void RemoveEffect(Character target)
    {
        target.speed += speedModifier;
    }
}