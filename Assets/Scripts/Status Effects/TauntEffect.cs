using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntEffect : StatusEffect
{
  
    public TauntEffect (float effectDuration)
    {
        effectName = "Taunt";
        
    }

    public override void ApplyEffect(Character tauntingCharacter)
    {
        if (tauntingCharacter == GameManager.Instance.companion1)
        {
            GameManager.Instance.probabilityCompanion1 = 1f;
            GameManager.Instance.probabilityCompanion2 = 0f;
            
        }
        if (tauntingCharacter == GameManager.Instance.companion2)
        {
            GameManager.Instance.probabilityCompanion1 = 0f;
            GameManager.Instance.probabilityCompanion2 = 1f;
            
        }
    }

    public override void RemoveEffect(Character target)
    {
            GameManager.Instance.probabilityCompanion1 = 1f;
            GameManager.Instance.probabilityCompanion2 = 1f;
    }

  

}
