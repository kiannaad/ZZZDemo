using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

public class CharacterStatus : NetworkBehaviour, IbeHurted, IStatus
{
    private Player _player;
    public float Doge_SlowTime;
    public float Doge_GapTime;

    public NetworkVariable<float> health { get; set; } = new NetworkVariable<float>(100);
    public Sprite HealdSprite;
    
    public void InvokeHealthChange(float _old, float _new)
    {
        if (IsLocalPlayer)
        {
            EventManager.Instance.SendEvent<HealthChangeEvent>(new HealthChangeEvent(_player.poolType,
                health.Value / maxHealth, HealdSprite));
        }
        
        if (IsClient && !IsLocalPlayer)
        {
            InvokeClientNoLocalHealthChange();
        }
    }
    public void InvokeClientNoLocalHealthChange()
    {
        //Debug.Log($"{transform.name}: InvokeClientNoLocalHealthChange");
        EventManager.Instance.SendEvent<ClientUIData>(new ClientUIData(HealdSprite, health.Value, NetworkObjectId));
    }
    [field : SerializeField] public float maxHealth { get; set; }
    public bool isDead { get; set; }
    
    public bool Invincible { get; set; }

    public void EnableInvincibility() => Invincible = true;
    public void DisableInvincibility() => Invincible = false;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += InvokeHealthChange;
    }

    private void Start()
    {
        Invincible = false;
        isDead = false;
    }

    public void OnHurted(Vector2 direction, float damage)
    {
        if (BulletTimeManager.Instance.isInBulletTime()) return;
        
        if (CheckForInvicible()) return;
        
        if (IsLocalPlayer)
        {
          ProcessDamageServerRpc(damage);
        }
    }

    [ServerRpc]
    private void ProcessDamageServerRpc(float damage)
    {
       health.Value -= damage;
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
        return health.Value - damage <= 0;
    }

    public void UIInit()
    {
        InvokeHealthChange(0, 0);
    }
    
}
