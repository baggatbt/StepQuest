using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class StatusEffectController : MonoBehaviour
{
    private Dictionary<Character, List<(StatusEffect effect, float duration)>> activeEffects = new Dictionary<Character, List<(StatusEffect, float)>>();

    public void AddEffect(StatusEffect effect, Character target, float duration)
    {
        if (!activeEffects.ContainsKey(target))
        {
            activeEffects[target] = new List<(StatusEffect, float)>();
        }
        
        effect.ApplyEffect(target);
        activeEffects[target].Add((effect, duration));
    }

    public void ProcessEffects()
    {
        foreach (var entry in activeEffects)
        {
            for (int i = entry.Value.Count - 1; i >= 0; i--)
            {
                var effectTuple = entry.Value[i];
                effectTuple.duration -= 1;

                if (effectTuple.duration <= 0)
                {
                    effectTuple.effect.RemoveEffect(entry.Key);
                    entry.Value.RemoveAt(i);
                }
                else
                {
                    entry.Value[i] = effectTuple; // Update the tuple with the decremented duration
                }
            }

            if (entry.Value.Count == 0)
            {
                activeEffects.Remove(entry.Key);
            }
        }
    }

    public void RemoveEffect(StatusEffect effect, Character target)
    {
        if (activeEffects.ContainsKey(target))
        {
            var effectsList = activeEffects[target];
            effectsList.RemoveAll(e => e.effect == effect);

            if (effectsList.Count == 0)
            {
                activeEffects.Remove(target);
            }
        }
    }
}
