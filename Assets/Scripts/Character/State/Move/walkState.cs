using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class walkState : MovementState
{
    public walkState( PlayerController player) : base(StateAction.walk, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.animator.CrossFadeInFixedTime("WalkStart", 0.14f);
    }

    public override void Update()
    {
        base.Update();
        player.UpdateVerticalRecenter(new Vector3(player.MoveInput.x, 0, player.MoveInput.z));
    }
    
    public override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        base.OnMoveCanceled(context);
        player.stateMachine.State = StateAction.idle;
        
    }
}
