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
    public bool canChain;
    public bool requiresMovement; // Indicates if the attack requires movement towards the target


    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);



    
}