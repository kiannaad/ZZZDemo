using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboState : GroundState
{
    public ComboState(StateAction state, PlayerController player) : base(state, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.recenteringSetting.DisableForHorizontalRecentering();
        player.recenteringSetting.DisableForVerticalRecentering();
    }
    
    public override void Update()
    {
        base.Update();
        //Debug.Log("ATK");
        player.UpdateRotation(player.content.AttackData.attackRotationTime, true, player.CheckEnemyIsValid());
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

    public override void OnLeftMouseStarted(InputAction.CallbackContext context)
    {
        if (player.ResuableDataAttack.canInput == false) return;

        Buffer_MoveToAttack();
    }
    
    public override void OnAnimationEnterEvent()
    {
        //Debug.Log("Enter");
        player.AttackAni_EnterSet();
    }

    public override void OnAnimationUpdateEvent()
    {
        player.AttackAni_UpdateSet();
    }
}
