using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : GroundState
{
    public IdleState(PlayerController player) : base(StateAction.idle, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetMovementZero();
    }

    public override void Update()
    {
        base.Update();
        player.UpdateVerticalRecenter(new Vector3(player.MoveInput.x, 0f, player.MoveInput.z));
        if (!player.notMoveInput())
        {
            player.stateMachine.State = StateAction.walk;
        }
    }
    
}
