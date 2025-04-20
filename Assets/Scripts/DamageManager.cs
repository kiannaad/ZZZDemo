using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public struct damageInfo
{
    public damageInfo(GameObject attacker, GameObject defender, float damage, Vector2 direction, int attackCount, Action<Transform> actions)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.damage = damage;
        this.direction = direction;
        this.onHit = actions;
        this.Attack_Count = attackCount;
    }
    public GameObject attacker;
    public GameObject defender;
    public float damage;
    public Vector2 direction;
    public int Attack_Count;
    public  Action<Transform> onHit;
}

public class DamageManager : MonoSigleton<DamageManager>
{
    private List<damageInfo> damageInfos = new List<damageInfo>();

    private void FixedUpdate()
    {
        if (damageInfos.Count == 0) return;
        damageInfos.RemoveAll(info =>
        {
            ProcessDamage(info);
            return true;
        });
    }

    private void ProcessDamage(damageInfo info)
    {
        IbeHurted hurted = info.defender.GetComponent<IbeHurted>();
        float damage = info.damage;
        Vector2 direction = info.direction;

        if (hurted == null) return;
        
        if (!hurted.CheckOnDied(damage))
        {
            hurted.OnHurted(direction, damage);
        }
        else
        {
            hurted.OnKilled(direction);
        }
        
        info.onHit?.Invoke(info.defender.transform);
    }

    public void AddDamage(damageInfo info)
    {
        int count = info.Attack_Count;
        while (count > 0)
        {
            damageInfos.Add(info);
            count--;
        }
       
    }
}
