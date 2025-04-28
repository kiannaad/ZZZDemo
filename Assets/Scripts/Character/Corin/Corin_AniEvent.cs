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
    
    public void ChangeToIdle() => player.controller.stateMachine.State = StateAction.idle;
    public void StartFinishCamera() => SwitchCamera.Instance.ImmediateSwitchToCamera(player.controller.ResuableDataAttack.finishSkillData.nameType, player.controller.ResuableDataAttack.finishSkillData.comboType);
    public void EndFinishCamera() => SwitchCamera.Instance.UnImmediateSwitchToCamera(player.controller.ResuableDataAttack.finishSkillData.nameType, player.controller.ResuableDataAttack.finishSkillData.comboType);

    public void UnActiveSelf()
    {
        player.gameObject.SetActive(false);
        player.DisableSwitching();
    } 
    
    public void EnableInvisible() => status.EnableInvincibility();
    public void DisableInvisible() => status.DisableInvincibility();
    
    public void SetForAttackCanInput() => player.controller.ResuableDataAttack.canInput = true;
    public void PlayFoot() => AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Corin, AudioClipType.foot);
}
