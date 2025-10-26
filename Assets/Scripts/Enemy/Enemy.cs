using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class Enemy : NetworkBehaviour, IDeflectable
{
    private Animator animator;
    public BehaviorTree behaviorTree;
    public GameObject player;
    public NavMeshAgent agent;
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
        if (IsServer)
        {
            OnDeflected = beDeflected;
            StartCoroutine(UpdateDistance());
            agent.enabled = true;
            behaviorTree.enabled = true;
        }
        
        if (IsClient)
        {
            CameraHitfeel.Instance.AddAni_Enemy(animator);
            agent.enabled = false;
            behaviorTree.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsServer) return;
        
        if (CheckCanRotate())
            RotateToPlayer();
        
        if (CanChase)
            SetForDestination();

        if (CompledtedDeflection.Value)
        {
            DisableCompledtedDeflection();
            OnDeflected?.Invoke();
        }
    }
    
    public void OnHit()
    {
        if (IsClient)
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
        if (player == null)
        {
            if (behaviorTree.GetVariable("Player").GetValue() as GameObject != null)
            {
                Debug.Log("player 赋值");
                player = behaviorTree.GetVariable("Player").GetValue() as GameObject;
            }
            else
            {
                Debug.Log("player return");
                return;
            }
        }
        GetDirectionToPlayer();
        var TargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, TargetAngle, transform.eulerAngles.z);
    }

    private void GetDirectionToPlayer()
    {
        if (player == null) Debug.LogError("player is null");
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
        if (!IsServer) return;
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
        if (player == null) return;
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

    public NetworkVariable<bool> CanbeDeflected { get; set; } = new NetworkVariable<bool>();
    public void EnableDeflection()
    {
        if (!IsServer) return;
        CanbeDeflected.Value = true;
    }
    public void DisableDeflection()
    {
        if (!IsServer) return;
        CanbeDeflected.Value = false;
    }

    public NetworkVariable<bool> CompledtedDeflection { get; set; } = new NetworkVariable<bool>();
    public void EnableCompledtedDeflection()
    {
        if (!IsServer) return;
        CompledtedDeflection.Value = true;
    }
    public void DisableCompledtedDeflection()
    {
        if (!IsServer) return;
        CompledtedDeflection.Value = false;
    }

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
