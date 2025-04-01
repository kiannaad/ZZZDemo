using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveNullState : IState
{
    public StateAction State { get; }
    public PlayerController player { get; set; }

    public moveNullState(PlayerController player)
    {
        this.player = player;
        this.State = StateAction.None;
    }
    
    public void Enter()
    {
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

    public void OnAnimationUpdateEvent()
    {
    }

    public void OnAnimationExitEvent()
    {
    }
}
