using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectDamagePassive : Skill
{
    public ReflectDamagePassive()
    {
        isActiveSkill = false; // This skill is passive
        // Other initializations...
    }

    public void Awake()
    {

    }

    
    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
       
        yield break; // No operation for passive skills
    }

     public override void ApplyPassiveEffect(CharacterData characterData)
    {
        characterData.damageReflectionPercentage = 0.25f; // Set the damage reflection percentage
        Debug.Log("Applied ReflectDamagePassive effect to " + characterData.heroID);
    }
        
}
