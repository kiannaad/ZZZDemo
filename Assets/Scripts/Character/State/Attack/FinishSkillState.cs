using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSkillState : ComboState
{
    public FinishSkillState(PlayerController player) : base(StateAction.FinishSkill, player)
    {
    }

    public override void Enter()
    {
        player.finalskillAtkCallback();
    }
    
}
