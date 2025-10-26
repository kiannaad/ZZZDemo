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

    public void ChangeToIdle()
    {
        if (!player.IsOwner) return;
        player.controller.stateMachine.State = StateAction.idle;
    }
    public void UnActiveSelf()
    {
        player.SetActiveRpc(false);
        player.DisableSwitching();
    }
    
    public void StartFinishCamera()
    {
        if (!player.IsOwner) return;
        SwitchCamera.Instance.ImmediateSwitchToCamera(player.controller.ResuableDataAttack.finishSkillData.nameType,
            player.controller.ResuableDataAttack.finishSkillData.comboType);
    }
    
    public void EndFinishCamera()
    {
        if (!player.IsOwner) return;
        SwitchCamera.Instance.UnImmediateSwitchToCamera(player.controller.ResuableDataAttack.finishSkillData.nameType,
            player.controller.ResuableDataAttack.finishSkillData.comboType);
    }
    
    public void EnableInvisible()
    {
        if (!player.IsOwner) return;
        status.EnableInvincibility();
    }
    public void DisableInvisible()
    {
        if (!player.IsOwner) return;
        status.DisableInvincibility();
    }

    public void FootAudioPlay()
    {
        if (!player.IsOwner) return;
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.foot);
    }
    public void Foot2AudioPlay()
    {
        if (!player.IsOwner) return;
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.foot2);
    }
    public void FootBackPlay()
    {
        if (!player.IsOwner) return;
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.footback);
    }
    
    public void SetForAttackCanInput()
    {
        if (!player.IsOwner) return;
        player.controller.ResuableDataAttack.canInput = true;
    }

    public void PlayAnbi_RuQiao()
    {
        if (!player.IsOwner) return;
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.安比入鞘);
    }
    public void PlayAnbi_ShouDao()
    {
        if (!player.IsOwner) return;
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, AudioClipType.安比收刀);
    }
    public void SetForAttackRotate()
    {
        if (!player.IsOwner) return;
        player.controller.ResuableDataAttack.canRotate = false;
    }

    public void AnBiVFX_Slash1()
    {
        //if (!player.IsOwner) return;
        player.fx.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash1);
    }
    public void AnBiVFX_Slash2()
    {
        //if (!player.IsOwner) return;
        player.fx.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash2);
    }

    public void AnBiVFX_Slash13()
    {
        //if (!player.IsOwner) return;
        player.fx.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash1);
        player.fx.PlayVFXItem(Character_Name.AnBi, VFXType.AnBi_slash3);
    }
}
