using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corin_AniEvent : MonoBehaviour
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
    public void StartFinishCamera()
    {
        if (!player.IsOwner) return;
        Debug.Log("StartFinishCamera");
        SwitchCamera.Instance.ImmediateSwitchToCamera(player.controller.ResuableDataAttack.finishSkillData.nameType,
            player.controller.ResuableDataAttack.finishSkillData.comboType);
    }
    public void EndFinishCamera()
    {
        if (!player.IsOwner) return;
        SwitchCamera.Instance.UnImmediateSwitchToCamera(player.controller.ResuableDataAttack.finishSkillData.nameType,
            player.controller.ResuableDataAttack.finishSkillData.comboType);
    }

    public void UnActiveSelf()
    {
        player.SetActiveRpc(false);
        player.DisableSwitching();
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
    
    public void SetForAttackCanInput()
    {
        if (!player.IsOwner) return;
        player.controller.ResuableDataAttack.canInput = true;
    }
    public void PlayFoot()
    {
        if (!player.IsOwner) return;
        AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Corin, AudioClipType.foot);
    }
}
