using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKState : ComboState
{
    public ATKState(PlayerController player) : base(StateAction.ATK, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("ATK State Enter");
        player.atkCallback();
        player.recenteringSetting.DisableForHorizontalRecentering();
        player.recenteringSetting.DisableForVerticalRecentering();
    }
}
