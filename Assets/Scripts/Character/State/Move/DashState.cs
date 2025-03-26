using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashState : GroundState
{
    public DashState(PlayerController player) : base(StateAction.dash, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player._curMoveInput = player.MoveInput;
        if (player.notMoveInput()) player.animator.CrossFadeInFixedTime("Dash_Back", 0.14f);
        else
        {
            player.animator.CrossFadeInFixedTime("Dash_Front", 0.14f);
            player.animator.SetBool(player.aniHarsh.HasInputID, true);
        }
        player.PlayWindAudio();
    }

    public override void Update()
    {
        base.Update();
        player.CheckForTurnBack();
    }
}
