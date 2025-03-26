using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementState : GroundState
{
    public MovementState(StateAction state, PlayerController player) : base(state, player)
    {
    }

    public override void Update()
    {
        base.Update();
        player.UpdateRotation(player.ResuableDataMove.smoothTime);
        player.Move(State);
    }
}
