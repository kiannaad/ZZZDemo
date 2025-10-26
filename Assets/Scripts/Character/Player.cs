using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityLayerMask;
using Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;


public interface IPlayer : ISwitch
{
    public GameObject owner { get; }
    public GameObject lookAt { get; set; }
    public Character_Name GetName();
    public void OnStartLocalPlayer();
    public void OnInit(ICharacterList list);
    public void OnAwake();
    public void OnStart();
    public void OnUpdate();
    public void OnFixedUpdate();
    public void EnableInput();
    public void DisableInput();
    public void SetActiveRpc(bool active);
    public void InvokeClientNoLocalHealthChange();
}

public class Player : NetworkBehaviour, IPlayer
{
    public Camera cam;
    public CinemachineVirtualCamera vcam;
    public Animator animator;
    private CharacterController characterController;
    public IStatus status;
    
    public CharacterSOData content;
    public PlayerController controller;
    public GameInput inputActions;
    private AnticipatedNetworkTransform anticipatedNetworkTransform;
    public VFXManager fx;

    public GameInput.PlayerInputActions InputActions
    {
        get
        {
            if (inputActions == null)
            {
                inputActions = new GameInput();
            }

            return inputActions.PlayerInput;
        }
    }
    private ICharacterList _list;
    private FlyingKnife _flyingKnife;
    
    public FlyingType flyingType;
    //public bool BulletTime;
    
    [field: SerializeField] public GameObject lookAt { get; set; }
    public float fadeTime;

    [field : SerializeField] public Character_Name poolType { get; private set; }

    private bool SwitchOuting = false;
    
    public void DisableSwitching() => SwitchOuting = false;
    
    public void EnableInput() => InputActions.Enable();
    public void DisableInput() => InputActions.Disable();
    public void InvokeClientNoLocalHealthChange() => status.InvokeClientNoLocalHealthChange();
    
    public Character_Name GetName() => poolType;
    public IStatus GetIStatus() => status;
    
    public void OnInit(ICharacterList list)
    {
        _list = list;
        fx = _list.fx;
    }

    public void OnAwake()
    {
        _flyingKnife = new FlyingKnife(content.AttackData.CheckDistance, LayerMask.GetMask("Enemy"), transform);
        animator = GetComponent<Animator>();
        _list = GetComponentInParent<CharacterList>();
        characterController = GetComponent<CharacterController>();
        status = GetComponent<IStatus>();
       // cam = Camera.main;
        controller = new PlayerController(content, this);
        anticipatedNetworkTransform = GetComponent<AnticipatedNetworkTransform>();
    }

    public GameObject owner => gameObject;

    public void OnStartLocalPlayer()
    {
        if (!IsLocalPlayer) return;
        
        //Debug.Log($"localPlayer: {gameObject.name}");
       
        /*EventManager.Instance.RegisterEvent<TimeLineStarted>(started =>
        {
            if (!gameObject.activeInHierarchy) return;

            Debug.Log("DisableInput");
            DisableInput();
            animator.enabled = false;
        });

        EventManager.Instance.RegisterEvent<TimeLineStopped>(stopped =>
        {
            if (!gameObject.activeInHierarchy) return;

            EnableInput();
            animator.Rebind();
            animator.enabled = true;
        });*/
        
        InputActions.Switch.performed += (context) =>
        {
            if (controller.stateMachine.State != StateAction.FinishSkill)
            {
                _list?.ChangeCharacter(context);
            }
        };

        EnableInput();
    }

    public void OnStart()
    {
        if (IsLocalPlayer)
            CameraHitfeel.Instance.AddAni_Player(animator);
        
        anticipatedNetworkTransform.enabled = false;
    }

    public void OnUpdate()
    {
        if (!IsLocalPlayer) return;
        
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            QTETimeManager.Instance.StartQTETime();
            /*animator.CrossFade("QuestStart", 0.14f);#1#
        }*/
        
        if (!SwitchOuting)
        {
            controller.Update();
        }
    }

    public void OnFixedUpdate()
    {
        if (!IsLocalPlayer) return;
        
        if (!SwitchOuting)
            controller.FixedUpdate();
    }

    private void OnAnimatorMove()
    {
        if (isForceChange) return;
        Vector3 move = animator.deltaPosition;
        //Quaternion rotation = animator.deltaRotation;
       
        characterController.Move(move);
        //transform.rotation *= rotation;
    }

    private bool isForceChange = false;
    
    [ServerRpc]
    public void SetFloatServerRpc(int nameId, float value) => animator.SetFloat(nameId, value);
    [ServerRpc]
    public void SetBoolServerRpc(int nameId, bool value)
    {
        animator.SetBool(nameId, value);
    }
    [ServerRpc]
    public void CrossFadeServerRpc(string nameId, float fadeTime)
    {
        animator.CrossFade(nameId, fadeTime);
    }
    [ServerRpc]
    public void CrossFadeInFixedTimeServerRpc(string name, float fadeTime)
    {
        animator.CrossFadeInFixedTime(name, fadeTime);
    }
   
    [ServerRpc]
    public void ChangePositionServerRpc(Vector3 pos)
    {
        /*transform.position = pos;
        ChangePosClientRpc(pos);*/
        anticipatedNetworkTransform.AnticipateMove(pos); 
    }

    [ClientRpc]
    private void ChangePosClientRpc(Vector3 pos)
    {
        transform.position = pos;
    }
    
    [ServerRpc]
    public void ChangeRotationServerRpc(Quaternion rot)
    {
        //ChangeRotClientRpc(rot);
        anticipatedNetworkTransform.AnticipateRotate(rot);
    }
    
    [ServerRpc]
    public void ChangePotServerRpc(Quaternion rot)
    {
        transform.rotation = rot;
        ChangeRotClientRpc(rot);
    }
    
    [ClientRpc]
    private void ChangeRotClientRpc(Quaternion rot)
    {
        transform.rotation = rot;
    }

    [ServerRpc]
    public void EnableForceChangeServerRpc() => EnableForceChangeClientRpc();
    [ClientRpc]
    public void EnableForceChangeClientRpc() => isForceChange = true;

    [ServerRpc]
    public void DisableForceChangeServerRpc() => DisableForceChangeClientRpc();
    
    [ClientRpc]
    public void DisableForceChangeClientRpc() => isForceChange = false;
    

    public void SetActiveRpc(bool active)
    {
        if (!IsLocalPlayer) return;
        gameObject.SetActive(active);
        SetSeverActiveServerRpc(active);
    }

    [ServerRpc]
    public void SetSeverActiveServerRpc(bool active)
    {
        SetActiveClientRpc(active);
        gameObject.SetActive(active);
    }
    
    [ClientRpc]
    private void SetActiveClientRpc(bool active) => gameObject.SetActive(active);

     #region ISwitch的具体实现

    private GameTimer timer = null;
    private Coroutine coroutine = null;

    private void cancelCoroutine()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    public void ChangeToHit() => controller.stateMachine.State = StateAction.Hit;
    public void Switch_In(Vector3 position, Vector3 rotation, Vector3 offset, bool isQTE = false)
    {
        Switch_In_Init();
        
        /*if (isQTE)
        {
            ChangePositionServerRpc(position);
            ChangeRotationServerRpc(Quaternion.Euler(rotation));
            Switch_In_QTE();
            return;
        }
        
        var enemy = _flyingKnife.TryGetCanFlyEnemy(transform.position);
       
        if (enemy != null)
        {
            //Debug.Log("flying success");
            ChangePositionServerRpc(position);
            ChangeRotationServerRpc(Quaternion.Euler(rotation));
            Switch_In_FlyingAction(enemy.transform.position, -enemy.transform.forward, offset);
        }
        else*/
        {
           Switch_In_Action(position, rotation, offset);
        }

        anticipatedNetworkTransform.enabled = false;
    }

    public void Switch_Out(Vector3 position, Vector3 rotation)
    {
        Switch_Out_Init(); 
        Switch_Out_Action(position, rotation);
        anticipatedNetworkTransform.enabled = false;
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

    private void Switch_In_QTE()
    {
        SetActiveRpc(true);
        DisableForceChangeServerRpc();
        controller.ChangeATKAction(() =>
        {
            CrossFadeServerRpc("QTEATK", 0.14f);
        },0.2f, 10, AudioClipType.安比技能, HitType.AnBi_Hit);
        controller.stateMachine.State = StateAction.ATK;
    }

    private void Switch_In_FlyingAction(Vector3 position, Vector3 Direction, Vector3 offset)
    {
        if (gameObject.activeInHierarchy) SetActiveRpc(false);
        
        ChangePositionServerRpc(position + -Direction.normalized * offset.magnitude);
        ChangeRotationServerRpc(Quaternion.LookRotation(Direction));
        
        SetActiveRpc(true);
        DisableForceChangeServerRpc();
        
        CrossFadeInFixedTimeServerRpc("FlyingCombo", 0.14f);
        
        if (flyingType == FlyingType.ATK)
        {
            _flyingKnife.FlyingAllEnemy();
        }
        else if (flyingType == FlyingType.Evade)
        {
            BulletTimeManager.Instance.StartBulletTime();
        }
    }
    
    private void Switch_In_Action(Vector3 position, Vector3 rotation, Vector3 offset)
    {
        if (gameObject.activeSelf)
        {
            Debug.Log($"当前角色已经是激活状态 Switch_In_Action");
            return;
        }
        
        ChangeRotationServerRpc(Quaternion.Euler(rotation));
        if (CheckCanSwitch_In(position, offset))
        {
            ChangePositionServerRpc(position + offset);
            //Debug.Log($"{transform.name} targetpos : {position + offset}, curpos : {transform.position}");
        }
        else
        {
            ChangePositionServerRpc(position);
        }
        
        SetActiveRpc(true);
        DisableForceChangeServerRpc();
        
        CrossFadeInFixedTimeServerRpc("SwitchIn", 0.14f);
        
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
        anticipatedNetworkTransform.enabled = true;
        EnableForceChangeServerRpc();

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
        coroutine = null;
        coroutine =  StartCoroutine(WaitForFade(position, rotation));
    }

    private void Switch_Out_Init()
    {
        SwitchOuting = true;
        anticipatedNetworkTransform.enabled = true;
        DisableInput();
        EnableForceChangeServerRpc();
    }
    
    private IEnumerator WaitForFade(Vector3 position, Vector3 rotation)
    {
        yield return new WaitUntil(() => controller.stateMachine.State != StateAction.FinishSkill &&
                                         controller.stateMachine.State != StateAction.Skill && 
                                         controller.stateMachine.State != StateAction.ATK);
        ChangePositionServerRpc(position);
        ChangeRotationServerRpc(Quaternion.Euler(rotation));
        
        DisableForceChangeServerRpc();
        
        CrossFadeInFixedTimeServerRpc("SwitchOut", 0.14f);
        
        FadeAway();
        //Debug.Log(poolType.ToString() + " " + controller.stateMachine.State);
    }
    
    private void FadeAway() => timer = TimerManager.Instance.GetTimer(fadeTime, () =>
    {
        if (gameObject.activeInHierarchy)
        {
            SetActiveRpc(false);
            DisableSwitching();
        }
        coroutine = null;
    });

    #endregion
    
}
