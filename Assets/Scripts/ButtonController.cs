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
        // Set the current skill of the player
        battleManager.player.currentSkill = new Slash();
        
        // Start the player's attack sequence
        battleManager.PlayerAttack();
    }
}


}
