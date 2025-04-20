using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour, IbeHurted, IStatus
{
    private Player _player;
    public float Doge_SlowTime;
    public float Doge_GapTime;
    public float Inviciability_MaxTime;
    public float health { get; set; }
    [field : SerializeField] public float maxHealth { get; set; }
    public bool isDead { get; set; }
    private float Invicibility_Timer { get; set; }
    
    public bool Invincible { get; set; }

    public void EnableInvincibility() => Invincible = true;
    public void DisableInvincibility() => Invincible = false;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        Invincible = false;
        isDead = false;
        health = maxHealth;
    }

    private void Update()
    {
        if (Invicibility_Timer > 0) Invicibility_Timer -= Time.deltaTime;
    }

    public void OnHurted(Vector2 direction, float damage)
    {
        if (Invicibility_Timer > 0) return;
        if (CheckForInvicible()) return;
        
        health -= damage;
        _player.ChangeToHit();
    }
    
    public void OnKilled(Vector2 direction)
    {
        if (Invicibility_Timer > 0) return;
        if (CheckForInvicible()) return;
        
        isDead = true;
        //Debug.Log("Player Killed");
    }
    
    private bool CheckForInvicible()
    {
        if (Invincible)
        {
            StartCoroutine(Dodge_VFX());
            Invicibility_Timer = Inviciability_MaxTime;
            return true;
        }

        return false;
    }

    public bool CheckOnDied(float damage)
    {
        return health - damage <= 0;
    }

    private IEnumerator Dodge_VFX()
    {
        Debug.Log("Dodge VFX");
        CameraHitfeel.Instance.slowTime(Doge_SlowTime);
        yield return new WaitForSeconds(Doge_GapTime);
        CameraHitfeel.Instance.slowTimeEnd();
    }
}
