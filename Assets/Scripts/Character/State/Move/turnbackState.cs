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
        player.animator.SetBool(player. aniHarsh.TurnBackID, true);
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log("TurnBack State");
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
        player.animator.SetBool(player.aniHarsh.TurnBackID, false);
    }
}
