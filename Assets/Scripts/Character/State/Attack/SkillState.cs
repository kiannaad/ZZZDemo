using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : ComboState
{
    public SkillState( PlayerController player) : base(StateAction.Skill, player)
    {
    }

    public override void Enter()
    {
        player.SkillATK();
    }
    
    
    
}
