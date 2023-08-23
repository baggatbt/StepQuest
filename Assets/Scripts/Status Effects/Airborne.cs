using UnityEngine;
using System.Collections;

public class Airborne : StatusEffect
{
    public float liftHeight = 5f; 

    public Airborne(float effectDuration)
    {
        effectName = "Airborne";
        duration = effectDuration;
    }

    public override void ApplyEffect(Character target)
    {
        Transform targetTransform = target.transform;
        Vector3 originalPosition = targetTransform.position;
        Vector3 liftedPosition = new Vector3(originalPosition.x, originalPosition.y + liftHeight, originalPosition.z);
        targetTransform.position = liftedPosition;
    }

    public override void RemoveEffect(Character target)
    {
        target.transform.position -= new Vector3(0, liftHeight, 0);
    }

    public override IEnumerator EffectCoroutine(Character target, StatusEffectController controller)
{
    yield return new WaitForSeconds(duration);
    controller.RemoveEffect(this, target);
}


}

/* USAGE /
    5.0f is the effect duration in this example,
    Airborne airborneEffect = new Airborne(5.0f); 
    statusEffectController.AddEffect(airborneEffect, targetCharacter);
*/