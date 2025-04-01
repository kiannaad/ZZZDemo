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
    
    public GameInput.PlayerInputActions playerActions;
    public Vector3 _curMoveInput;
    
    public Vector3 MoveInput{get; private set;}

    public StateAction moveAction { get;private set; }


    public void SetMovementZero() => animator.SetFloat(aniHarsh.MovementID, 0f);
    public void MoveToggle() => moveAction = moveAction == StateAction.walk ? StateAction.run : StateAction.walk;
    public bool notMoveInput() => MoveInput.x == 0 && MoveInput.z == 0;
    private void GetInput() => MoveInput = new Vector3(playerActions.Move.ReadValue<Vector2>().x, 0, playerActions.Move.ReadValue<Vector2>().y);
    
    public PlayerController(CharacterSOData content, Player _player)
    {
        moveAction = StateAction.walk;
        
        this.content = content;
        player = _player;
        vcam = _player.vcam;
        
        stateMachine = new FSM();
        ResuableDataMove = new ResuableData_Move();
        ResuableDataAttack = new ResuableData_Attack();
        aniHarsh = new AniHarsh();
        recenteringSetting = new RecenteringSetting(vcam);
        
        ResuableDataAttack.comboData = content.AttackData.LightCombos;
        ResuableDataAttack.skillData = content.AttackData.SkillCombo;
        ResuableDataAttack.finishSkillData = content.AttackData.FinishSKillCombo;
        
        cam = player.cam;
        animator = player.animator;
        this.playerActions = player.PlayerActions;
        
        stateMachine.AddState(new IdleState(this));
        stateMachine.AddState(new walkState(this));
        stateMachine.AddState(new DashState(this));
        stateMachine.AddState(new runState(this));
        stateMachine.AddState(new turnbackState(this));
        stateMachine.AddState(new moveNullState(this));
        stateMachine.AddState(new ATKState(this));
        stateMachine.AddState(new SkillState(this));
        stateMachine.AddState(new FinishSkillState(this));
    
        
    }

    public void Update()
    {
        GetInput();
        stateMachine.Update();
        CheckEnemyInDistance();
        //Debug.Log("CanInput" + ResuableDataAttack.canInput);
        //Debug.Log(ResuableDataAttack.comboCount);
    }

    public void FixedUpdate()
    {
        stateMachine.FixedUpdate(); 
    }

    /// <summary>
    /// 用来更新walk和run对应的速度上限
    /// </summary>
    /// <param name="action"></param>
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
    
    public Vector3 GetInputDirection() => Quaternion.Euler(0f, GetInputRotation(MoveInput), 0f) * Vector3.forward;

    #region Rotation

    /// <summary>
    /// 更新人物角度
    /// </summary>
    /// 旋转的时间
    /// <param name="smoothTime"></param>
    /// 是否要面向那个物体，比如敌人
    /// <param name="Istoward"></param>
    /// 面向的具体物体
    /// <param name="towardObject"></param>
    public void UpdateRotation(float smoothTime, bool Istoward = false, GameObject towardObject = null)
    {
        if (notMoveInput() && !Istoward) return;
        if (Istoward && towardObject == null) return;
        
        float Targetrotation ;
        if (Istoward)
        {
            Vector3 tar = new Vector3(Mathf.Clamp(towardObject.transform.position.x - player.transform.position.x, -1, 1), 
                0f, Mathf.Clamp(towardObject.transform.position.z  - player.transform.position.z, -1, 1));
            
            Targetrotation = GetInputRotation(tar, true);
        }
        else
        {
            Targetrotation = GetInputRotation(MoveInput);
        }
        
        float RotateToTarget = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, Targetrotation, ref ResuableDataMove.rotateVelocity, smoothTime);
        
        player.transform.rotation = Quaternion.Euler(0, RotateToTarget, 0);
    }

    /// <summary>
    /// 更新输入后需要旋转的角度
    /// </summary>
    /// 输入值
    /// <param name="input"></param>
    /// 是否是面向某个物体
    /// <param name="isToward"></param>
    /// <returns></returns>
    private float GetInputRotation(Vector3 input, bool isToward = false)
    {
        float rotation = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;

        if (rotation < 0)
        {
            rotation += 360;
        }
        
        if (!isToward)
            AddCameraRotation(ref rotation);
        
        return rotation;
    }

    /// <summary>
    /// 添加摄像机的旋转
    /// </summary>
    /// <param name="rotation"></param>
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
    /// <summary>
    /// 查看是否可以转身
    /// </summary>
    public void CheckForTurnBack()
    {
        if (notMoveInput()) return;

        //Debug.Log(_curMoveInput != MoveInput);
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
    
    /// <summary>
    /// 更新水平回中，根据俯仰角来判断
    /// </summary>
    /// <param name="moveInput"></param>
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

    /// <summary>
    /// 更新垂直回中，根据俯仰角来判断
    /// </summary>
    /// <param name="moveInput"></param>
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
        AudioClipPoolManager.Instance.PlayAudioClip(Characterlist.AnBi, AudioClipType.Wind);
}
