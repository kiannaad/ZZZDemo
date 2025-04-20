using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : GroundState
{
    public HitState(PlayerController player) : base(StateAction.Hit, player)
    {
    }

    public override void Enter()
    {
        player.SetMovementZero();
        player.Hited();
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void OnAnimationEnterEvent()
    {
    }

    public override void OnAnimationUpdateEvent()
    {
    }

    public override void OnAnimationExitEvent()
    {
        player.stateMachine.State = StateAction.idle;
    }
}
