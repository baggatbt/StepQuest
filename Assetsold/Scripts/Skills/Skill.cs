using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct AttackStage
{
    public string animationTrigger;
    public float timingWindowStart;
    public float timingWindowEnd;
    public int damage;
}

public abstract class Skill
{
    public string name;
    public string description;
    public bool skillExecutionComplete; // Flag to track the completion of skill execution 
    public List<AttackStage> attackStages;

    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);

    public IEnumerator MoveToTarget(Character attacker, Character target, float duration)
{
    Vector3 originalPosition = attacker.transform.position;
    Vector3 targetPosition = target.transform.position;

    // Offset the target position to move to the left edge of the target sprite
    targetPosition.x -= target.spriteRenderer.bounds.extents.x + attacker.spriteRenderer.bounds.extents.x;

    float timer = 0.0f;
    while (timer <= duration)
    {
        float step = timer / duration;
        attacker.transform.position = Vector3.Lerp(originalPosition, targetPosition, step);
        timer += Time.deltaTime;
        yield return null;
    }

    // Update attacker's current position after moving
    attacker.currentPosition = attacker.transform.position;
}




    public IEnumerator MoveBack(Character attacker, float duration)
    {
        Vector3 targetPosition = attacker.originalPosition;
        Vector3 currentPosition = attacker.transform.position;

        float timer = 0.0f;
        while (timer <= duration)
        {
            float step = timer / duration;
            attacker.transform.position = Vector3.Lerp(currentPosition, targetPosition, step);
            timer += Time.deltaTime;
            yield return null;
        }
    }


}