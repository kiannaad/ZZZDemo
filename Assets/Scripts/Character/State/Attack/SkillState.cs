using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : IState
{
    public SkillState(PlayerController player)
    {
        this.player = player;
        State = StateAction.Skill;
    }
    public StateAction State { get; }
    public PlayerController player { get; set; }
    public void Enter()
    {
        player.SkillATK();
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }

    public void AddInputAction()
    {
        
    }

    public void RemoveInputAction()
    {
       
    }

    public void OnAnimationEnterEvent()
    {
       
    }

    public void OnAnimationExitEvent()
    {
        player.stateMachine.State = StateAction.idle;
    }
}
