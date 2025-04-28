using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class Enemy : MonoBehaviour, IDeflectable
{
    private Animator animator;
    public BehaviorTree behaviorTree;
    public GameObject player;
    private NavMeshAgent agent;
    private CharacterController characterController;
    public GroundContact groundContact;

    private float distance;
    public float damage;

    public float smoothTime;
    private Vector3 direction;
    private float targetAngle;
    public float offsetAngle;
    private float currentVelocity;

    public bool CanChase =false;

   
    private void Awake()
    {
        animator = GetComponent<Animator>();
        behaviorTree = GetComponent<BehaviorTree>();
        agent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        groundContact = GetComponent<GroundContact>();
    }

    private void Start()
    {
        EventManager.Instance.RegisterEvent<OnCharacterSwitch>(@switch => ChangePlayer(@switch.curCharacter));
        OnDeflected = beDeflected;
        StartCoroutine(UpdateDistance());
        CameraHitfeel.Instance.AddAni_Enemy(animator);
    }

    private void Update()
    {
        //Debug.Log($"animator speed: {animator.speed}");
        if (CheckCanRotate())
            RotateToPlayer();
        
        if (CanChase)
            SetForDestination();

        if (CompledtedDeflection)
        {
            DisableCompledtedDeflection();
            OnDeflected?.Invoke();
        }
    }

    private void ChangePlayer(GameObject player) => this.player = player;
    
    public void OnHit()
    {
        AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Enemy, AudioClipType.punch8);
    }

    #region 更新距离

    private IEnumerator UpdateDistance()
    {
        while (true)
        {
            if (player != null)
            {
                distance = Vector3.Distance(player.transform.position, transform.position);
                behaviorTree.SetVariableValue("Distance", distance);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

    #region Rotation

    private void RotateToPlayer()
    {
        GetDirectionToPlayer();
        var TargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, TargetAngle, transform.eulerAngles.z);
    }

    private void GetDirectionToPlayer()
    {
        direction = player.transform.position - transform.position;
        targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (targetAngle < 0)
        {
            targetAngle += 360;
        }
    }

    private bool CheckisInAngle()
    {
        float curAngle = transform.eulerAngles.y;
        if (curAngle < targetAngle + offsetAngle && curAngle > targetAngle - offsetAngle) return true;
        return false;
    }

    private bool CheckCanRotate()
    {
        var a = animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(!CheckisInAngle());
        bool checkTag = a.IsTag("Idle") || a.IsTag("Walk");
        //Debug.Log(!CheckisInAngle());
        if (distance < 50f && !CheckisInAngle() && checkTag)
        {
            //Debug.Log("Enemy Rotate");
            return true;
        }

        return false;
    }

    public void ForceToFacePlayer()
    {
        if (player == null) return;
        Vector3 dir = Vector3.Normalize(player.transform.position - transform.position);

        transform.forward = new Vector3(dir.x, 0f, dir.z).normalized;
       // Debug.Log("ForceToFacePlayer");

    }

    #endregion

    #region Move

    public bool canAttackRotate = false;
    [FormerlySerializedAs("canRunRotate")] public bool canRunAgent = false;

    private void OnAnimatorMove()
    {
        if (canAttackRotate)
        {
            ForceToFacePlayer();
            canAttackRotate = false;
            return;
        }

        if (canRunAgent)
        {
            return;
        }

        Vector3 move = animator.deltaPosition;
        Quaternion rotation = animator.deltaRotation;
        
        characterController.Move(move);
        transform.rotation *= rotation;
    }

    private void SetForDestination()
    {
        agent.SetDestination(player.transform.position);

        if (distance <= agent.stoppingDistance)
        {
            animator.SetTrigger("stopRun");
        }
        /*Debug.Log($"SetForDestination {distance <= agent.stoppingDistance}");*/
    }
    
    public void ResetAgent() => agent.ResetPath();

    #endregion

    public Action OnDeflected { get; set; }

    public bool CanbeDeflected { get; set; }
    public void EnableDeflection() => CanbeDeflected = true;
    public void DisableDeflection() => CanbeDeflected = false;
    public bool CompledtedDeflection { get; set; }
    public void EnableCompledtedDeflection() => CompledtedDeflection = true;
    public void DisableCompledtedDeflection() => CompledtedDeflection = false;

    public void beDeflected()
    {
        StartCoroutine(EnterDeflected());
    }

    private IEnumerator EnterDeflected()
    {
        Debug.Log("EnterDeflected");
        DisableDeflection();
        behaviorTree.enabled = false;
        animator.CrossFadeInFixedTime("Stun_Hit_H_Front", 0.14f);
        yield return new WaitForSeconds(1f);
        behaviorTree.enabled = true;
    }
}
