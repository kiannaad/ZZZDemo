using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyState : NetworkBehaviour, IbeHurted
{
    private Enemy _enemy;
    private Animator animator;
    public NetworkVariable<float> health = new NetworkVariable<float>(100);
    public bool isDead = false;

    public string deadth_Front;
    public string deadth_Back;
    public string deadth_stay;
    
    public string HitBackAni_Name;
    public string HitFrontAni_Name;
    public string HitStayAni_Name;
    
    public string Hit_ShakeAni_Name;


    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Debug.Log($"{health.Value} enemy");
    }

    public void OnHurted(Vector2 direction, float damage)
    {
        OnHurtedServerRpc(direction, damage);
        beAttackedServerRpc(direction, damage);
    }

    public void OnKilled(Vector2 direction)
    {
        OnKillServerRpc(direction);
    }

    public bool CheckOnDied(float damage) => health.Value  - damage <= 0;
    
    public bool Behaviour_CheckDie() => health.Value  <= 0;
    
    private IEnumerator SpecialHurted()
    {
        _enemy.behaviorTree.enabled = false;
        animator.CrossFadeInFixedTime("Stun_Hit_H_Front", 0.14f);
        yield return new WaitForSeconds(2f);
        _enemy.behaviorTree.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnKillServerRpc(Vector2 direction)
    {
        if (isDead) return;
        Debug.Log("Enemy OnKilled");
        health.Value = 0f;
        _enemy.agent.enabled = false;
        _enemy.behaviorTree.enabled = false;
        if (Vector2.Angle(transform.forward, direction) < 70f) animator.CrossFadeInFixedTime(deadth_Front, 0.14f);
        if (Vector2.Angle(-transform.forward, direction) < 70f) animator.CrossFadeInFixedTime(deadth_Back, 0.14f);
        else animator.CrossFadeInFixedTime(deadth_stay, 0.14f);
        isDead = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void beAttackedServerRpc(Vector2 direction, float damage)
    {
        var a = animator.GetCurrentAnimatorStateInfo(0);
        if (!(a.IsTag("Idle") || a.IsTag("Walk") || a.IsTag("Hit"))) return;
        animator.CrossFadeInFixedTime(Hit_ShakeAni_Name, 0.14f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnHurtedServerRpc(Vector2 direction, float damage)
    {
        if (isDead) return;
        health.Value -= damage;
    }
}
