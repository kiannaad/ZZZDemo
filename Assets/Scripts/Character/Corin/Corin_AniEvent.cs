using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corin_AniEvent : MonoBehaviour
{
    private Player player;
    private CharacterList list;

    
    private void Awake()
    {
        player = GetComponent<Player>();
        list = GetComponentInParent<CharacterList>();
    }
    
    public void ChangeToIdle() => player.controller.stateMachine.State = StateAction.idle;

    public void UnActiveSelf()
    {
        player.gameObject.SetActive(false);
        player.DisableSwitching();
    } 
    
    public void SetForAttackCanInput() => player.controller.ResuableDataAttack.canInput = true;
    public void PlayFoot() => AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Corin, AudioClipType.foot);
}
