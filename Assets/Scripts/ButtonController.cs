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
        // Set the current skill of the player: Change this to not be hard coded to a skill eventually
        battleManager.player.currentSkill = new TripleHitSkill();
        
        // Start the player's attack sequence
        battleManager.PlayerAttack();
    }
}


}
