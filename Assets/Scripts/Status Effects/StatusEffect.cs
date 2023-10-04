using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StatusEffect
{
    public string effectName;
    

    public abstract void ApplyEffect(Character target);
    public abstract void RemoveEffect(Character target);
    
    

/*
        // Create an instance of Airborne with a duration of 2 seconds
        Airborne airborneEffect = new Airborne(1.0f);
        battleManager.statusEffectController.AddEffect(airborneEffect, target);

        */
}

