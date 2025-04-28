using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HealthChangeEvent
{
    public HealthChangeEvent(Character_Name name, float curHealthPersent, Sprite headSprite)
    {
        this.name = name;
        this.curHealthPersent = curHealthPersent;
        this.head = headSprite;
    }
    public Character_Name name { get; private set; }
    public float curHealthPersent { get; private set; }
    public Sprite head { get; private set; }
}

public class CharacterStatus : MonoBehaviour, IbeHurted, IStatus
{
    private Player _player;
    public float Doge_SlowTime;
    public float Doge_GapTime;
    
    private float Health;
    public Sprite HealdSprite;
    public float health
    {
        get
        {
            return Health;
        }
        set
        {
            if (value == Health) return;
            Health = value;
            InvokeHealthChange();
        }
    }
    
    public void InvokeHealthChange() => EventManager.Instance.SendEvent<HealthChangeEvent>(new HealthChangeEvent(_player.poolType, health / maxHealth, HealdSprite));
    
    [field : SerializeField] public float maxHealth { get; set; }
    public bool isDead { get; set; }
    
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

    public void OnHurted(Vector2 direction, float damage)
    {
        if (BulletTimeManager.Instance.isInBulletTime()) return;
        
        if (CheckForInvicible()) return;
        
        health -= damage;
        _player.ChangeToHit();
    }
    
    public void OnKilled(Vector2 direction)
    {
        if (BulletTimeManager.Instance.isInBulletTime()) return;
        
        if (CheckForInvicible()) return;
        
        isDead = true;
        //Debug.Log("Player Killed");
    }
    
    private bool CheckForInvicible()
    {
        if (Invincible)
        {
            BulletTimeManager.Instance.StartBulletTime();
            return true;
        }

        return false;
    }

    public bool CheckOnDied(float damage)
    {
        return health - damage <= 0;
    }

    public void UIInit()
    {
        InvokeHealthChange();
    }
    
}
