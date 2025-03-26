using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class runState : MovementState
{
    
    public runState( PlayerController player) : base(StateAction.run, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player._curMoveInput = player.MoveInput;
    }
    
    public override void Update()
    {
        base.Update();
        player.CheckForTurnBack();
        player.UpdateVerticalRecenter(new Vector3(player.MoveInput.x, 0, player.MoveInput.z));
        //Debug.Log("Run State");
    }
    

    public override void OnMoveStarted(InputAction.CallbackContext context)
    {
        player.animator.SetBool(player.aniHarsh.HasInputID, true);
    }
    
}
