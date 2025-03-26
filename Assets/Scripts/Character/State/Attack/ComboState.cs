using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboState : GroundState
{
    public ComboState(StateAction state, PlayerController player) : base(state, player)
    {
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log($"canRotate: {player.ResuableDataAttack.canRotate}");
        if (player.ResuableDataAttack.canRotate)
            player.UpdateRotation(player.content.AttackData.attackRotationTime);
    }
    public override void OnMoveStarted(InputAction.CallbackContext context)
    {
        player.animator.SetBool(player.aniHarsh.HasInputID, true);
        if (player.ResuableDataAttack.canMoveInterrupt == false) return;
        player.stateMachine.State = player.moveAction;
    }
    
    public override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        player.animator.SetBool(player.aniHarsh.HasInputID, false);
        
        if (player.ResuableDataAttack.canMoveInterrupt == false) return;
        
        Buffer_MoveToIdle();
        player.recenteringSetting.DisableForHorizontalRecentering();
    }

    public override void OnDashStarted(InputAction.CallbackContext context)
    {
        
        player.stateMachine.State = StateAction.dash;
        player.curAttackSound.gameObject.SetActive(false);
        player.ResetComboData();
    }

    public override void OnDashCanceled(InputAction.CallbackContext context)
    {
        if (player.notMoveInput()) player.stateMachine.State = StateAction.idle;
        else
        {
            player.stateMachine.State = StateAction.run;
        }
    }

    public override void OnMouseStarted(InputAction.CallbackContext context)
    {
        if (player.ResuableDataAttack.canInput == false) return;

        Buffer_MoveToAttack();
    }
}
