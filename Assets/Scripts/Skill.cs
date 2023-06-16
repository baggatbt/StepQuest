using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Skill
{
    public string name;
    public string description;


    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);
}
