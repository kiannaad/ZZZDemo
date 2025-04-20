using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    public BehaviorTree behaviorTree;
    public GameObject player;
    private NavMeshAgent agent;
    private CharacterController characterController;

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
    }

    private void Start()
    {
        CharacterList.Instance.OnCharacterSelected += ChangePlayer;
        this.player = CharacterList.Instance.GetCurPlayer();
        StartCoroutine(UpdateDistance());
    }

    private void Update()
    {
        if (CheckCanRotate())
            RotateToPlayer();
        
        if (CanChase)
            SetForDestination();
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
        Debug.Log("ForceToFacePlayer");

    }

    #endregion

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
}
