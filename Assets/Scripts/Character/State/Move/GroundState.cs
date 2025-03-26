using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class GroundState : IState
{
    public StateAction State { get; }
    public PlayerController player { get; set; }
    private GameTimer _gameTimer1;
    private GameTimer _gameTimer2;
    public GroundState(StateAction state, PlayerController player)
    {
        State = state;
        this.player = player;
    }
    
    public  virtual void Enter()
    {
        //Debug.Log($"Enter {State}");
        AddInputAction();
    }

    public  virtual void Update()
    {
        //Debug.Log($"Update {State}");
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
        //Debug.Log($"Exit {State}");
        RemoveInputAction();
    }

    public virtual void AddInputAction()
    {
        player.PlayerActions.Move.started += OnMoveStarted;
        player.PlayerActions.Move.canceled += OnMoveCanceled;
        player.PlayerActions.Dash.started += OnDashStarted;
        player.PlayerActions.Dash.canceled += OnDashCanceled;
        player.PlayerActions.LeftMouse.started += OnMouseStarted;
        player.PlayerActions.Skill.started += OnSkillStarted;
        
        player.PlayerActions.Pointer.started -= UnRegisterBuffer_MoveToIdle;

        player.PlayerActions.Pointer.performed += context => player.UpdateMoveRecenter(new Vector2(player.MoveInput.x, player.MoveInput.z));
        player.PlayerActions.Move.performed += context => player.UpdateMoveRecenter(context.ReadValue<Vector2>());
    }

    public virtual void RemoveInputAction()
    {
        player.PlayerActions.Move.started -= OnMoveStarted;
        player.PlayerActions.Move.canceled -= OnMoveCanceled;
        player.PlayerActions.Dash.started -= OnDashStarted;
        player.PlayerActions.Dash.canceled -= OnDashCanceled;
        player.PlayerActions.LeftMouse.started -= OnMouseStarted;
        player.PlayerActions.Skill.started -= OnSkillStarted;
    }

    public virtual void OnAnimationEnterEvent()
    {
    }

    public virtual void OnAnimationExitEvent()
    {
    }

    public virtual void OnMoveStarted(InputAction.CallbackContext context)
    {
        player.stateMachine.State = player.moveAction;
        player.animator.SetBool(player.aniHarsh.HasInputID, true);
    }

    public virtual void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Buffer_MoveToIdle();
        player.recenteringSetting.DisableForHorizontalRecentering();
    }

    public virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        player.stateMachine.State = StateAction.dash;
    }

    public virtual void OnDashCanceled(InputAction.CallbackContext context)
    {
        if (player.notMoveInput()) player.stateMachine.State = StateAction.idle;
        else
        {
            player.stateMachine.State = StateAction.run;
        }
    }

    public virtual void OnMouseStarted(InputAction.CallbackContext context)
    {
        Buffer_MoveToAttack();
    }

    public virtual void OnSkillStarted(InputAction.CallbackContext context)
    {
        player.stateMachine.State = StateAction.Skill;
    }

    public virtual void Buffer_MoveToIdle()
    {
        _gameTimer1 = TimerManager.Instance.GetTimer(player.ResuableDataMove.BufferTime_MoveToIdle, () =>
        {
            player.stateMachine.State = StateAction.idle;
            player.animator.SetBool(player.aniHarsh.HasInputID, false);
        });
       
        player.PlayerActions.Move.started += UnRegisterBuffer_MoveToIdle;
    }

    public virtual void Buffer_MoveToAttack()
    {
        _gameTimer2 = TimerManager.Instance.GetTimer(player.ResuableDataMove.BufferTime_MoveToAttacking, () =>
        {
            player.stateMachine.State = StateAction.ATK;
            player.ATK();
        });
    }
    
    public virtual void UnRegisterBuffer_MoveToIdle(InputAction.CallbackContext context)
    {
        TimerManager.Instance.UnRigisterTimer(_gameTimer1);
    }
}
