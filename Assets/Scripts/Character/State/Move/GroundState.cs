using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Utilities;

public class GroundState : IState
{
    public StateAction State { get; }
    public PlayerController player { get; set; }
    private GameTimer _gameTimer1;
    private GameTimer _gameTimer2;
    private GameTimer _gameTimer3;
    public GroundState(StateAction state, PlayerController player)
    {
        State = state;
        this.player = player;
    }
    
    public  virtual void Enter()
    {
       //Debug.Log($"{player.player.poolType.ToString()} Enter {State}");
        AddInputAction();
    }

    public  virtual void Update()
    {
       //Debug.Log($"{player.player.poolType.ToString()} Update {State}");
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
        //Debug.Log($"{player.player.poolType.ToString()} Exit {State}");
        RemoveInputAction();
    }

    public virtual void AddInputAction()
    {
        player.playerInputActions.Move.started += OnMoveStarted;
        player.playerInputActions.Move.canceled += OnMoveCanceled;
        player.playerInputActions.Dash.started += OnDashStarted;
        player.playerInputActions.Dash.canceled += OnDashCanceled;
        player.playerInputActions.LeftMouse.performed += OnLeftMousePerformed;
        player.playerInputActions.Skill.started += OnSkillStarted;
        player.playerInputActions.FinishSkill.started += OnFinishSkillStarted;
        
        player.playerInputActions.Pointer.started -= UnRegisterBuffer_MoveToIdle;

        player.playerInputActions.Pointer.performed += context => player.UpdateMoveRecenter(new Vector2(player.MoveInput.x, player.MoveInput.z));
        player.playerInputActions.Move.performed += context => player.UpdateMoveRecenter(context.ReadValue<Vector2>());
    }

    public virtual void RemoveInputAction()
    {
        player.playerInputActions.Move.started -= OnMoveStarted;
        player.playerInputActions.Move.canceled -= OnMoveCanceled;
        player.playerInputActions.Dash.started -= OnDashStarted;
        player.playerInputActions.Dash.canceled -= OnDashCanceled;
        player.playerInputActions.LeftMouse.performed -= OnLeftMousePerformed;
        player.playerInputActions.Skill.started -= OnSkillStarted;
        player.playerInputActions.FinishSkill.started -= OnFinishSkillStarted;
    }

    public virtual void OnAnimationEnterEvent()
    {
    }

    public virtual void OnAnimationUpdateEvent()
    {
    }

    public virtual void OnAnimationExitEvent()
    {
    }
    
    public virtual void OnMoveStarted(InputAction.CallbackContext context)
    {
        player.stateMachine.State = player.moveAction;
        player.SetBool(player.aniHarsh.HasInputID, true);
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
    
    public virtual void OnLeftMousePerformed(InputAction.CallbackContext context)
    {
       if (context.interaction is TapInteraction)
       {
           Buffer_MoveToAttack();
       }
       else if (context.interaction is HoldInteraction && player.isSpecialatk_hold)
       {
           Buffer_MoveToAttack();
       }
    }
    
    public virtual void OnSkillStarted(InputAction.CallbackContext context)
    {
        player.stateMachine.State = StateAction.Skill;
    }
    
    public virtual void OnFinishSkillStarted(InputAction.CallbackContext context)
    {
        player.stateMachine.State = StateAction.FinishSkill;
    }
    
    public virtual void Buffer_MoveToIdle()
    {
        _gameTimer1 = TimerManager.Instance.GetTimer(player.content.moveData.BufferTime_MoveToIdle, () =>
        {
            player.stateMachine.State = StateAction.idle;
            player.SetBool(player.aniHarsh.HasInputID, false);
        });
       
        player.playerInputActions.Move.started += UnRegisterBuffer_MoveToIdle;
    }
    
    public virtual void Buffer_MoveToAttack()
    {
        _gameTimer2 = TimerManager.Instance.GetTimer(player.content.moveData.BufferTime_MoveToAttacking, () =>
        {
            //Debug.Log("To Atk");
            player.stateMachine.State = StateAction.ATK;
        });
    }
    
    public virtual void Buffer_DashToMove()
    {
        _gameTimer3 = TimerManager.Instance.GetTimer(player.content.moveData.BufferTime_DashToMove, () =>
        {
            player.stateMachine.State = StateAction.run;
        });
    }
    
    public virtual void UnRegisterBuffer_MoveToIdle(InputAction.CallbackContext context)
    {
        TimerManager.Instance.UnRigisterTimer(_gameTimer1);
    }
}
