using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnbackState : MovementState
{
    public turnbackState(PlayerController player) : base(StateAction.turnBack, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log(player.animator.GetBool(player.aniHarsh.TurnBackID));
        player.SetBool(player. aniHarsh.TurnBackID, true);
    }

    public override void Update()
    {
        player.UpdateRotation(0.01f);
    }

    public override void OnAnimationEnterEvent()
    {
        //Debug.Log("On Animation Transition Event");
        if (player.notMoveInput()) player.stateMachine.State = StateAction.idle;
        else
        {
            player.stateMachine.State = StateAction.run;
        }
    }

    public override void Exit()
    {
        base.Exit();
        //Debug.Log("TurenBack Exit");
        player.SetBool(player.aniHarsh.TurnBackID, false);
    }
}