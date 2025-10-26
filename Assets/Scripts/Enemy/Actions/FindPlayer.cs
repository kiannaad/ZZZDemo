using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Enemy")]
[TaskDescription("使用盒型检测在指定范围内搜索最近的玩家并设置为目标")]
public class FindPlayer : Action
{
    [Header("搜索设置")]
    public float searchRadius = 100f;
    public string playerTag = "Player";
    public float searchCooldown = 2f;
    public bool requireLineOfSight = true;
    
    [Header("盒型检测设置")]
    public Vector3 boxSize = new Vector3(50f, 50f, 50f);
    public bool useWorldOrientation = true;
    
    [Header("输出变量")]
    public SharedGameObject targetPlayer;
    public SharedBool hasTarget;
    public SharedFloat distanceToTarget;
    
    private Enemy enemy;
    private NavMeshAgent agent;
    private float lastSearchTime;
    
    public override void OnStart()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        lastSearchTime = -searchCooldown;
    }
    
    public override TaskStatus OnUpdate()
    {
        if (Time.time - lastSearchTime < searchCooldown)
        {
            return TaskStatus.Running;
        }
     
        lastSearchTime = Time.time;
        
        GameObject foundPlayer = SearchForPlayerWithBox();
        
        if (foundPlayer != null)
        {
            SetTarget(foundPlayer);
            return TaskStatus.Success;
        }
        else
        {
            ClearTarget();
            return TaskStatus.Failure;
        }
    }
    
    private GameObject SearchForPlayerWithBox()
    {
        Vector3 center = Owner.transform.position;
        Vector3 halfExtents = boxSize * 0.5f;
        Quaternion orientation = useWorldOrientation ? Quaternion.identity : transform.rotation;
        
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, orientation);
        GameObject closestPlayer = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider collider in hitColliders)
        {
            Debug.Log(collider.gameObject.name);
            if (IsValidPlayer(collider.gameObject))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                
                if (distance <= searchRadius && distance < closestDistance)
                {
                    if (requireLineOfSight && !HasLineOfSight(collider.gameObject))
                    {
                        continue;
                    }
                    
                    closestDistance = distance;
                    closestPlayer = collider.gameObject;
                }
            }
        }
        
        return closestPlayer;
    }
    
    private bool IsValidPlayer(GameObject playerObject)
    {
        if (playerObject == null) return false;
        if (!playerObject.CompareTag(playerTag)) return false;
        
        /*NetworkObject netObj = playerObject.GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsSpawned)
        {
            Debug.Log("NetObj 有问题跳过");
            return false;
        }*/
        
        return true;
    }
    
    private bool HasLineOfSight(GameObject target)
    {
        if (target == null) return false;
        
        RaycastHit hit;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.transform.position);
        
        Vector3 rayStart = transform.position + Vector3.up * 1f;
        Vector3 targetPoint = target.transform.position + Vector3.up * 0.5f;
        
        if (Physics.Raycast(rayStart, (targetPoint - rayStart).normalized, out hit, distance))
        {
            return hit.collider.gameObject == target;
        }
        
        return true;
    }
    
    private void SetTarget(GameObject newTarget)
    {
        if (newTarget == null) return;
        
        targetPlayer.Value = newTarget;
        hasTarget.Value = true;
        distanceToTarget.Value = Vector3.Distance(transform.position, newTarget.transform.position);
        
        if (agent != null && !agent.enabled)
        {
            agent.enabled = true;
        }
        
        Debug.Log($"盒型检测找到目标玩家: {newTarget.name}, 距离: {distanceToTarget.Value:F1}");
    }
    
    private void ClearTarget()
    {
        targetPlayer.Value = null;
        hasTarget.Value = false;
        distanceToTarget.Value = float.MaxValue;
    }
    
    public override void OnEnd()
    {
        // 清理工作
    }
}
