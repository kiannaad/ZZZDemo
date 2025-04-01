using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateAction
{
    idle, 
    walk,
    run,
    dash,
    turnBack,
    ATK,
    Skill,
    FinishSkill,
    None,
    size
}

public interface IState
{
    public StateAction State { get; }
    public PlayerController player { get; set; }
    
    public void Enter();
    public void Update();
    public void FixedUpdate();
    public void Exit();

    public void AddInputAction();
    public void RemoveInputAction();

    public void OnAnimationEnterEvent();
    public void OnAnimationUpdateEvent();
    public void OnAnimationExitEvent();
}

public class FSM
{
    private StateAction curstate = StateAction.None;
    private StateAction prestate = StateAction.None;
    public IState[] states;

    public FSM()
    {
        states = new IState[(int)StateAction.size];
    }
    
    public void AddState(IState state) => states[(int)state.State] = state;

    public void Update()
    {
        if (curstate == StateAction.None) return;
        
        states[(int)curstate].Update();
    }

    public void FixedUpdate()
    {
        if (curstate == StateAction.None) return;
        
        states[(int)curstate].FixedUpdate();
    }

    public StateAction State
    {
        set
        {
            //攻击状态需要重复进入，状态重复进入是需求，意味着需要重复利用，如果出现这个情况应该清除多余的状态转换。
            //if (curstate == value) return;

            prestate = curstate;
            curstate = value;

            if (prestate != StateAction.None)
            {
                states[(int)prestate].Exit();
            }
            
            states[(int)curstate].Enter();
        }
        
        get => curstate;
    }
}
