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
           // Debug.Log(other.transform.name);
            damageInfo info = _player.controller.ResuableDataAttack.damageInfo;
            CameraHitfeel.Instance.ShakeCamera(info.shakeForce);
            GameManager.Instance.HurtProcess(info, other.gameObject);
        }
    }
}
