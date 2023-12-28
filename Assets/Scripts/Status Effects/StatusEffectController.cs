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

        Debug.Log($"Added effect {effect.effectName} to {target.name} for {duration} turns.");
    }

    public void ProcessEffects()
    {
        foreach (var entry in activeEffects)
        {
            for (int i = entry.Value.Count - 1; i >= 0; i--)
            {
                var effectTuple = entry.Value[i];
                effectTuple.duration -= 1;

                Debug.Log($"Processing effect {effectTuple.effect.effectName} on {entry.Key.name}. Duration remaining: {effectTuple.duration}");

                if (effectTuple.duration <= 0)
                {
                    effectTuple.effect.RemoveEffect(entry.Key);
                    entry.Value.RemoveAt(i);

                    Debug.Log($"Effect {effectTuple.effect.effectName} on {entry.Key.name} has ended.");
                }
                else
                {
                    entry.Value[i] = effectTuple; // Update the tuple with the decremented duration
                }
            }

            if (entry.Value.Count == 0)
            {
                activeEffects.Remove(entry.Key);

                Debug.Log($"All effects on {entry.Key.name} have been processed and removed.");
            }
        }
    }

    public void RemoveEffect(StatusEffect effect, Character target)
    {
        if (activeEffects.ContainsKey(target))
        {
            var effectsList = activeEffects[target];
            effectsList.RemoveAll(e => e.effect == effect);

            Debug.Log($"Effect {effect.effectName} manually removed from {target.name}.");

            if (effectsList.Count == 0)
            {
                activeEffects.Remove(target);

                Debug.Log($"All effects on {target.name} have been manually removed.");
            }
        }
    }
}