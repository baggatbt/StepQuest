using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Knight : PlayerJob
{
    private int playerLevel => PlayerData.Instance.level;

    public override int BaseAtk => 3 * playerLevel;
    public override int BaseDef => 4 * playerLevel;  
    public override int BaseMagAtk => 2 * playerLevel;  
    public override int BaseMagDef => 2 * playerLevel;  
    public override int BaseHealth => 10 + (playerLevel + 2);
    public override int BaseEnergy => 5;
}


    

 

