using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy_collider : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            Action<Transform> action = (t) => _player.controller.PlayHitResource(t);
            DamageManager.Instance.AddDamage(new damageInfo(_player.gameObject, other.gameObject, 
                2f,  _player.transform.forward, 1, action));
        }
    }
}
