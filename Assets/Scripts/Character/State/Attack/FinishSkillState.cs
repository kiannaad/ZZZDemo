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
        Debug.Log("Finish Skill Enter");
        player.FinishSkillATK();
        SwitchCamera.Instance.ImmediateSwitchToCamera(player.ResuableDataAttack.finishSkillData.nameType, player.ResuableDataAttack.finishSkillData.comboType);
    }

    public override void Exit()
    {
        base.Exit();
        SwitchCamera.Instance.UnImmediateSwitchToCamera(player.ResuableDataAttack.finishSkillData.nameType, player.ResuableDataAttack.finishSkillData.comboType);
    }
    
}
