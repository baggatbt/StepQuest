using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StatusEffect
{
    public string effectName;
    

    public abstract void ApplyEffect(Character target);
    public abstract void RemoveEffect(Character target);
    
}

