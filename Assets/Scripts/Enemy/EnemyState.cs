using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour, IbeHurted
{
    private Enemy _enemy;
    private Animator animator;
    public float health;
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

    public void OnHurted(Vector2 direction, float damage)
    {
        if (isDead) return;
        
        health -= damage;
        beAttacked(direction, damage);
        //Debug.Log(health);
    }

    public void OnKilled(Vector2 direction)
    {
        if (isDead) return;
        
        health = 0f;
        _enemy.behaviorTree.enabled = false;
        if (Vector2.Angle(transform.forward, direction) < 70f) animator.CrossFadeInFixedTime(deadth_Front, 0.14f);
        if (Vector2.Angle(-transform.forward, direction) < 70f) animator.CrossFadeInFixedTime(deadth_Back, 0.14f);
        else animator.CrossFadeInFixedTime(deadth_stay, 0.14f);
        isDead = true;
        //Debug.Log("Enemy Die");
    }

    public bool CheckOnDied(float damage) => health - damage <= 0;
    
    public bool Behaviour_CheckDie() => health <= 0;
    
    public void beAttacked(Vector2 direction, float damage)
    {
        //Debug.Log("Attacked");
        var a = animator.GetCurrentAnimatorStateInfo(0);
        if (!(a.IsTag("Idle") || a.IsTag("Walk") || a.IsTag("Hit"))) return;

        //_enemy.behaviorTree.enabled = false;
        animator.CrossFadeInFixedTime(Hit_ShakeAni_Name, 0.14f);
        /*Vector2 curDirection = new Vector2(transform.forward.x, transform.forward.z).normalized;
        var angle = Vector2.Angle(curDirection, direction);
        if (angle > 180f) angle = 360f - angle;
        // Debug.Log($"angle: {angle}");
        if (angle > 90f) animator.CrossFadeInFixedTime(HitFrontAni_Name, 0.14f);
        else if (angle <= 90f) animator.CrossFadeInFixedTime(HitBackAni_Name, 0.14f);*/
    }
    
}
