using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StatusEffect
{
    public string effectName;
    public float duration;

    public abstract void ApplyEffect(Character target);
    public abstract void RemoveEffect(Character target);
    public abstract IEnumerator EffectCoroutine(Character target, StatusEffectController controller);
}

