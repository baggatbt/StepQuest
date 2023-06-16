using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public BattleManager battleManager;

    public void OnButtonClick()
    {
        // Only initiate an attack if it's the player's turn and neither the player nor the enemy is currently attacking
        if (!battleManager.player.isAttacking && !battleManager.enemy.isAttacking)
        {
            // Create an instance of TripleHitSkill
            TripleHitSkill tripleHitSkill = new TripleHitSkill();

            // Use the TripleHitSkill
            battleManager.UseSkill(tripleHitSkill, battleManager.player, battleManager.enemy);
        }
    }
}
