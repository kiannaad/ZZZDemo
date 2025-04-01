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

    public void ChangeToIdle() => player.controller.stateMachine.State = StateAction.idle;

    public void FootAudioPlay() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.foot);
    public void FootBackPlay() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.footback);
    
    public void SetForAttackCanInput() => player.controller.ResuableDataAttack.canInput = true;

    public void PlayAnbi_RuQiao() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.安比入鞘);
    public void PlayAnbi_ShouDao() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.安比收刀);
    public void SetForAttackRotate() => player.controller.ResuableDataAttack.canRotate = false;

    public void AnBiVFX_Slash1() => VFXManager.Instance.GetVFXItem(Characterlist.AnBi, VFXType.AnBi_slash1);
    public void AnBiVFX_Slash2() => VFXManager.Instance.GetVFXItem(Characterlist.AnBi, VFXType.AnBi_slash2);

    public void AnBiVFX_Slash13()
    {
        VFXManager.Instance.GetVFXItem(Characterlist.AnBi, VFXType.AnBi_slash1);
        VFXManager.Instance.GetVFXItem(Characterlist.AnBi, VFXType.AnBi_slash3);
    }
}
