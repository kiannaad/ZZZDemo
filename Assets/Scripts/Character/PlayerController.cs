using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public partial class PlayerController
{
    public FSM stateMachine;
    public Player player;
    
    
    public ResuableData_Move ResuableDataMove;
    public CharacterSOData content;
    public ResuableData_Attack ResuableDataAttack;
    
    public AniHarsh aniHarsh;
    public RecenteringSetting recenteringSetting;
    
    
    public Camera cam;
    public CinemachineVirtualCamera vcam;
    public Animator animator;
    
    public GameInput.PlayerInputActions PlayerActions;
    public Vector3 _curMoveInput;
    
    public Vector3 MoveInput{get; private set;}

    public StateAction moveAction { get;private set; }


    public void SetMovementZero() => animator.SetFloat(aniHarsh.MovementID, 0f);
    public void MoveToggle() => moveAction = moveAction == StateAction.walk ? StateAction.run : StateAction.walk;
    public bool notMoveInput() => MoveInput.x == 0 && MoveInput.z == 0;
    private void GetInput() => MoveInput = new Vector3(PlayerActions.Move.ReadValue<Vector2>().x, 0, PlayerActions.Move.ReadValue<Vector2>().y);
    
    public PlayerController(CharacterSOData content, Player _player)
    {
        moveAction = StateAction.walk;
        
        this.content = content;
        player = _player;
        vcam = _player.vcam;
        
        stateMachine = new FSM();
        ResuableDataMove = new ResuableData_Move();
        aniHarsh = new AniHarsh();
        recenteringSetting = new RecenteringSetting(vcam);
        
        ResuableDataAttack = new ResuableData_Attack();
        ResuableDataAttack.comboData = content.AttackData.LightCombos;
        ResuableDataAttack.skillData = content.AttackData.SkillCombo;
        
        cam = player.cam;
        animator = player.animator;
        PlayerActions = player.PlayerActions;
        
        stateMachine.AddState(new IdleState(this));
        stateMachine.AddState(new walkState(this));
        stateMachine.AddState(new DashState(this));
        stateMachine.AddState(new runState(this));
        stateMachine.AddState(new turnbackState(this));
        stateMachine.AddState(new moveNullState(this));
        stateMachine.AddState(new ATKState(this));
        stateMachine.AddState(new SkillState(this));
    
        stateMachine.State = StateAction.idle;
    }

    public void Update()
    {
        GetInput();
        stateMachine.Update();
    }

    public void FixedUpdate()
    {
        stateMachine.FixedUpdate(); 
    }

    public void Move(StateAction action)
    {
        var targetSpeed = action switch
        {
            StateAction.walk => content.moveData.walkSpeed,
            StateAction.run => content.moveData.runSpeed,
            _ => 0f
        };

        var TargetToSpeed = Mathf.SmoothDamp(animator.GetFloat(aniHarsh.MovementID), targetSpeed,
            ref ResuableDataMove.MoveSpeed, content.moveData.moveTime);
        
        animator.SetFloat(aniHarsh.MovementID, TargetToSpeed);
    }
    
    public Vector3 GetInputDirection() => Quaternion.Euler(0f, GetInputRotation(), 0f) * Vector3.forward;

    #region Rotation

    public void UpdateRotation(float smoothTime)
    {
        if (notMoveInput()) return;
        
        float Targetrotation = GetInputRotation();
        
        float RotateToTarget = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, Targetrotation, ref ResuableDataMove.rotateVelocity, smoothTime);
        
        player.transform.rotation = Quaternion.Euler(0, RotateToTarget, 0);
    }

    private float GetInputRotation()
    {
        float rotation = Mathf.Atan2(MoveInput.x, MoveInput.z) * Mathf.Rad2Deg;

        if (rotation < 0)
        {
            rotation += 360;
        }
        
        AddCameraRotation(ref rotation);
        
        return rotation;
    }

    private void AddCameraRotation(ref float rotation)
    {
        float cameraRotation = cam.transform.eulerAngles.y;
        rotation += cameraRotation;
        if (rotation > 360)
        {
            rotation -= 360;
        }
    }

    #endregion

    #region CheckForTurnBack

    public void CheckForTurnBack()
    {
        if (notMoveInput()) return;


        if (_curMoveInput != MoveInput)
        {
            if (_curMoveInput == -MoveInput)
            {
                _curMoveInput = MoveInput;
                stateMachine.State = StateAction.turnBack;
            }
            else
            {
                _curMoveInput = MoveInput;
            }
        }
    }

    #endregion

    #region 摄像机回中设置

    public void UpdateMoveRecenter(Vector2 moveInput)
    {
        
        if (moveInput == Vector2.up || moveInput == Vector2.down)
        {
            recenteringSetting.DisableForHorizontalRecentering();
            return;
        }
        
        float cameraEuler = cam.transform.eulerAngles.x;
        if (cameraEuler > 90)
        {
            cameraEuler -= 360;
        }
        
        cameraEuler = Mathf.Abs(cameraEuler);
        
        // CheckForVerticalRecentering(cameraEuler);
       
        if (moveInput == Vector2.left || moveInput == Vector2.right) CheckForHorizontalRecentering(cameraEuler);
    }

    public void UpdateVerticalRecenter(Vector2 moveInput)
    {
        float cameraEuler = cam.transform.eulerAngles.x;
        if (cameraEuler > 90)
        {
            cameraEuler -= 360;
        }
        cameraEuler = Mathf.Abs(cameraEuler);
        CheckForVerticalRecentering(cameraEuler);
    }

    private void CheckForHorizontalRecentering(float cameraEuler)
    {
        //if (recenteringSetting.SetForCancelHorizontal(cameraEuler)) return;
        
        foreach (RecenteringData recenteringData in player.content.sideRecenterData)
        {
            if (!recenteringData.isinAngle(cameraEuler)) continue;
            
            recenteringSetting.SetForHorizontalRecentering(recenteringData.waitTime, recenteringData.recenterTime);
            return;
        }
        
        recenteringSetting.DisableForHorizontalRecentering();
    }

    private void CheckForVerticalRecentering(float cameraEuler)
    {
        //Debug.Log(cameraEuler);
        if (recenteringSetting.SetForCancelVertical(cameraEuler)) return;
        
        foreach (RecenteringData recenteringData in player.content.backRecenterData)
        {
            if (!recenteringData.isinAngle(cameraEuler)) continue;
            
            recenteringSetting.SetForVerticalRecentering(recenteringData.waitTime, recenteringData.recenterTime);
            return;
        }
        
        //recenteringSetting.DisableForVerticalRecentering();
    }

    #endregion

    public void PlayWindAudio() =>
        AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, AudioClipType.Wind);
}
