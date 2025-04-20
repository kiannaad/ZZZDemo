using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DamageManager.Instance.AddDamage(
                new damageInfo(_enemy.gameObject, other.gameObject, _enemy.damage, 
                    new Vector2(_enemy.transform.forward.x, _enemy.transform.forward.z), 1,
                    transform1 =>
                    {
                        _enemy.OnHit();
                    } ));
        }
    }
}
