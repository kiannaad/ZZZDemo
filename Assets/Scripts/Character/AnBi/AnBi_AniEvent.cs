using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnBi_AniEvent : MonoBehaviour
{
    private Player player;
    private CharacterList list;
    private CharacterStatus status;

    private void Awake()
    {
        player = GetComponent<Player>();
        list = GetComponentInParent<CharacterList>();
        status = GetComponentInParent<CharacterStatus>();
    }

    public void ChangeToIdle() => player.controller.stateMachine.State = StateAction.idle;
    public void UnActiveSelf() 
    {
        player.gameObject.SetActive(false);
        player.DisableSwitching();
    }

    public void EnableInvisible() => status.EnableInvincibility();
    public void DisableInvisible() => status.DisableInvincibility();

    public void FootAudioPlay() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.foot);
    public void Foot2AudioPlay() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.foot2);
    public void FootBackPlay() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.footback);
    
    public void SetForAttackCanInput() => player.controller.ResuableDataAttack.canInput = true;

    public void PlayAnbi_RuQiao() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.安比入鞘);
    public void PlayAnbi_ShouDao() => AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.安比收刀);
    public void SetForAttackRotate() => player.controller.ResuableDataAttack.canRotate = false;

    public void AnBiVFX_Slash1() => VFXManager.Instance.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash1);
    public void AnBiVFX_Slash2() => VFXManager.Instance.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash2);

    public void AnBiVFX_Slash13()
    {
        VFXManager.Instance.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash1);
        VFXManager.Instance.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash3);
    }
}
