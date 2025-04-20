using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, ISwitch
{
    
    public Camera cam;
    public CinemachineVirtualCamera vcam;
    public Animator animator;
    private CharacterController characterController;
    public IStatus status;
    
    public CharacterSOData content;
    public PlayerController controller;
    public GameInput.PlayerInputActions inputActions;
    private CharacterList list;
    [field: SerializeField] public GameObject lookAt { get; private set; }
    public float fadeTime;

    public Character_Name poolType;

    private bool SwitchOuting = false;
    
    public void DisableSwitching() => SwitchOuting = false;
    
    public void EnableInput() => inputActions.Enable();
    public void DisableInput() => inputActions.Disable();
    
    private void Awake()
    {
        GameInput input = new GameInput();
        inputActions = input.PlayerInput;
        EnableInput();
        
        animator = GetComponent<Animator>();
        controller = new PlayerController(content, this);
        list = GetComponentInParent<CharacterList>();
        characterController = GetComponent<CharacterController>();
        status = GetComponent<IStatus>();
    }

    private void Start()
    {
        SwitchIn_Action += Switch_In_Action;
        SwitchOut_Action += Switch_Out_Action;
        
        inputActions.Switch.performed += (context) =>
        {
            if (controller.stateMachine.State != StateAction.FinishSkill)
            {
                list?.ChangeCharacter(context);
            }
        };
    }

    private void Update()
    {
        if (!SwitchOuting)
        {
            controller.Update();
        }
    }

    private void FixedUpdate()
    {
        if (!SwitchOuting)
            controller.FixedUpdate();
    }

    private void OnAnimatorMove()
    {
        Vector3 move = animator.deltaPosition;
        Quaternion rotation = animator.deltaRotation;
        
        characterController.Move(move);
        transform.rotation *= rotation;
    }

    #region ISwitch的具体实现

    private GameTimer timer = null;
    private Coroutine coroutine = null;

    private Action<Vector3, Vector3, Vector3> SwitchIn_Action;
    private Action<Vector3, Vector3> SwitchOut_Action;

    private void cancelCoroutine()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    public void ChangeToHit() => controller.stateMachine.State = StateAction.Hit;
    public void Switch_In(Vector3 position, Vector3 rotation, Vector3 offset)
    {
        Switch_In_Init();
        
        if (gameObject.activeSelf) return;
        
        SwitchIn_Action?.Invoke(position, rotation, offset);
    }

    public void Switch_Out(Vector3 position, Vector3 rotation)
    {
        Switch_Out_Init();

        SwitchOut_Action?.Invoke(position, rotation);
    }
    
    public SwitchType CanSwitch()
    {
        if (status == null) Debug.Log("No status found");
        if (status.isDead) return SwitchType.Next;
        if (SwitchOuting && (controller.stateMachine.State != StateAction.Skill && 
                             controller.stateMachine.State != StateAction.ATK)) return SwitchType.Wait;
        return SwitchType.immediate;
    }

    #endregion

    #region 进场的行为方法

    private bool CheckCanSwitch_In(Vector3 position, Vector3 offset)
    {
        var isGround = Physics.Raycast(position, offset.normalized, offset.magnitude, 
            LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);
        if (isGround) return false;
        return true;
    }
    private void Switch_In_Action(Vector3 position, Vector3 rotation, Vector3 offset)
    {
        transform.rotation = Quaternion.Euler(rotation);
        if (CheckCanSwitch_In(position, offset))
            transform.position = position + offset;
        else
        {
            transform.position = position;
        }
        
        gameObject.SetActive(true);
        
        animator.CrossFadeInFixedTime("SwitchIn", 0.14f);
        
        if (controller.CheckEnemyIsValid() != null)
        {
            controller.stateMachine.State = StateAction.ATK;
        }
    }

    private void Switch_In_Init()
    {
        TimerManager.Instance.UnRigisterTimer(timer);
        cancelCoroutine();
        SwitchOuting = false;

        TimerManager.Instance.GetTimer(0.14f, () =>
        {
            EnableInput();
        });
        
        controller.stateMachine.State = StateAction.idle;
    }

    #endregion

    #region 退场的行为方法

    private void Switch_Out_Action(Vector3 position, Vector3 rotation)
    {
        coroutine =  StartCoroutine(WaitForFade(position, rotation));
    }

    private void Switch_Out_Init()
    {
        SwitchOuting = true;
        DisableInput();
    }
    
    private IEnumerator WaitForFade(Vector3 position, Vector3 rotation)
    {
        yield return new WaitUntil(() => controller.stateMachine.State != StateAction.FinishSkill &&
                                         controller.stateMachine.State != StateAction.Skill && 
                                         controller.stateMachine.State != StateAction.ATK);
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
        animator.CrossFadeInFixedTime("SwitchOut", 0.14f);
        
        FadeAway();
        //Debug.Log(poolType.ToString() + " " + controller.stateMachine.State);
    }
    
    private void FadeAway() => timer = TimerManager.Instance.GetTimer(fadeTime, () =>
    {
        gameObject.SetActive(false);
        DisableSwitching();
    });

    #endregion
}
