using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class StatusEffectController : MonoBehaviour
{
    private List<StatusEffect> activeEffects = new List<StatusEffect>();
    private Dictionary<StatusEffect, Coroutine> effectCoroutines = new Dictionary<StatusEffect, Coroutine>();

    public void AddEffect(StatusEffect effect, Character target)
    {
        if (!HasEffect(effect.effectName))
        {
            activeEffects.Add(effect);
            effect.ApplyEffect(target);
            var coroutine = StartCoroutine(effect.EffectCoroutine(target, this));
            effectCoroutines[effect] = coroutine;
        }
    }

    public void RemoveEffect(StatusEffect effect, Character target)
    {
        if (HasEffect(effect.effectName))
        {
            if (effectCoroutines.ContainsKey(effect))
            {
                StopCoroutine(effectCoroutines[effect]);
                effectCoroutines.Remove(effect);
            }
            effect.RemoveEffect(target);
            activeEffects.Remove(effect);
        }
    }

    public bool HasEffect(string effectName)
    {
        return activeEffects.Any(effect => effect.effectName == effectName);
    }
}