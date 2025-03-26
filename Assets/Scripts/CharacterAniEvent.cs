using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAniEvent : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void FootAudioPlay() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.foot);
    public void FootBackPlay() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.footback);
    
    public void SetForAttackCanInput() => player.controller.ResuableDataAttack.canInput = true;

    public void PlayAnbi_ShouDao() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.安比入鞘);
    public void SetForAttackRotate() => player.controller.ResuableDataAttack.canRotate = false;
}
