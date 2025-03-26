using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKState : ComboState
{
    public ATKState(PlayerController player) : base(StateAction.ATK, player)
    {
    }
    public override void OnAnimationEnterEvent()
    {
        //Debug.Log("Enter");
        player.ResetComboData();

        if (!player.notMoveInput())
        {
            player.stateMachine.State = StateAction.walk;
            player.curAttackSound.gameObject.SetActive(false);
        }
    }

    public override void OnAnimationExitEvent()
    {
        base.OnAnimationExitEvent();
        player.stateMachine.State = StateAction.idle;
        player.curAttackSound.gameObject.SetActive(false);
       // Debug.Log("attack ani exit");
       
    }
}
