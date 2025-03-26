using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    public SoundItem curAttackSound;
    public void ATK()
    {
        ComboData data = ResuableDataAttack.comboData[ResuableDataAttack.comboCount];
        string name = data.comboName;
        animator.CrossFade(name, 0.111f);

        curAttackSound = AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.sweaponSound);
        AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.CharacterSounds);
        
        AddComboCount();

        ResuableDataAttack.canInput = false;
        ResuableDataAttack.canMoveInterrupt = false;
        ResuableDataAttack.canRotate = true;
    }

    public void SkillATK()
    {
        ComboData data = ResuableDataAttack.skillData;
        string name = data.comboName;
        animator.CrossFade(name, 0.111f);

        AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.sweaponSound);
        AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.CharacterSounds);
    }

    private void AddComboCount()
    {
        ResuableDataAttack.comboCount++;
        if (ResuableDataAttack.comboCount > ResuableDataAttack.comboData.Count - 1)
        {
           // Debug.Log("count exceeded");
            ResuableDataAttack.comboCount = 0;
        }
    }

    public void ResetComboData()
    {
        //Debug.Log("ResetComboData");
        ResuableDataAttack.comboCount = 0;
        ResuableDataAttack.canRotate = true;
        ResuableDataAttack.canInput = true;
        ResuableDataAttack.canMoveInterrupt = true;
    }
}
