using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    public Action atk;
    public Action skill_atk;
    public Action finalskill_atk;

    //public bool isSpecialATK_tap = false;
    public bool isSpecialatk_hold = false;

    public float ATKReset_ColdTime;
    
    public void SetForResetColdTime(float time) => ATKReset_ColdTime = time;
    
    private void InitAtkAction()
    {
        RegisterATK(ATK);
        RegisterSkill(SkillATK);
        RegisterFinalSkill(FinishSkillATK);
    }

    public void ChangeATKAction(Action action, float time, float damage, AudioClipType type, HitType vfxType, bool isSpecialHold = false)
    {
        RegisterATK(action);
        SetForResetColdTime(time);
        this.isSpecialatk_hold = isSpecialHold;
        UpdatehitResource(damage, type, vfxType, 0.01f);
    }

    public void atkCallback() => atk?.Invoke();
    public void skillAtkCallback() => skill_atk?.Invoke();
    public void finalskillAtkCallback() => finalskill_atk?.Invoke();
    
    public void RegisterATK(Action atk) => this.atk = atk;
    public void ReverseATK() => RegisterATK(ATK);
    public void RegisterSkill(Action skill) => this.skill_atk += skill;
    public void RegisterFinalSkill(Action finalskill) => this.finalskill_atk += finalskill;
    public void UnregisterSkill(Action skill) => this.skill_atk -= skill;
    public void UnregisterFinalSkill(Action finalskill) => this.finalskill_atk -= finalskill;
    
    public List<SoundItem> curAttackSound = new List<SoundItem>();
    private struct hitResource
    {
        public float damage;
        public AudioClipType hitSFX;
        public HitType hitVFX;
        public float ShakeForce;
    }

    private hitResource hit;
    
    /// <summary>
    /// 连击采用的数据应用封装
    /// </summary>
    public void ATK()
    {
        //Debug.Log("ATK");
        curAttackSound?.Clear();
        ComboData data = ResuableDataAttack.comboData[ResuableDataAttack.comboCount];
        string name = data.comboName;
        player.CrossFadeServerRpc(name, 0.111f);
        UpdatehitResource(data.damage, data.HitSounds, data.HitVFX, data.ShakeForce);
       // Debug.Log(data.comboName);
        curAttackSound.Add(AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.sweaponSound));
        curAttackSound.Add(AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.CharacterSounds));
        
        ATKSet();
        AddComboCount();
        SetForResetColdTime(data.coldTime);
    }

    /// <summary>
    /// 小技能采用的数据应用封装
    /// </summary>
    public void SkillATK()
    {
        curAttackSound?.Clear();
        ComboData data = ResuableDataAttack.skillData;
        string name = data.comboName;
        player.CrossFadeServerRpc(name, 0.111f);
        UpdatehitResource(data.damage, data.HitSounds, data.HitVFX, data.ShakeForce);
        curAttackSound.Add(AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.sweaponSound));
        curAttackSound.Add(AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.CharacterSounds));
       
        ATKSet();
        SetForResetColdTime(data.coldTime);
    }

    /// <summary>
    /// 大招采用的数据应用封装
    /// </summary>
    public void FinishSkillATK()
    {
        curAttackSound?.Clear();
        ComboData data = ResuableDataAttack.finishSkillData;
        string name = data.comboName;
        player.CrossFadeServerRpc(name, 0.111f);
        UpdatehitResource(data.damage, data.HitSounds, data.HitVFX, data.ShakeForce);
        //AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.sweaponSound);
        //AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.CharacterSounds);
        SwitchCamera.Instance.ImmediateSwitchToCamera(player.controller.ResuableDataAttack.finishSkillData.nameType,
            player.controller.ResuableDataAttack.finishSkillData.comboType);
        ATKSet();
        SetForResetColdTime(data.coldTime);
    }

    public void SoundClear()
    {
        if (curAttackSound.Count == 0) return;
        foreach (var item in curAttackSound)
        {
            if (item != null)
                item.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ATKSet()
    {
        ResuableDataAttack.canInput = false;
        ResuableDataAttack.canMoveInterrupt = false;
        //Debug.Log("ATKSet");
    }

    private void UpdatehitResource(float damage, AudioClipType hitSFX, HitType hitVFX, float shakeForce)
    {
        hit.hitSFX = hitSFX;
        hit.hitVFX = hitVFX;
        hit.damage = damage;
        hit.ShakeForce = shakeForce;
    }

    public void PlayHitResource(Transform transform)
    {
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, hit.hitSFX);
        player.fx.PlayHitVFXItem(player.poolType, hit.hitVFX, transform);
        CameraHitfeel.Instance.ShakeCamera(hit.ShakeForce);
        CameraHitfeel.Instance.PS(0.01f);
    }

    /// <summary>
    /// 连击计数
    /// </summary>
    private void AddComboCount()
    {
        ResuableDataAttack.comboCount++;
        if (ResuableDataAttack.comboCount > ResuableDataAttack.comboData.Count - 1)
        {
            // Debug.Log("count exceeded");
            ResuableDataAttack.comboCount = 0;
        }
    }

    /// <summary>
    /// 重置默认连击数据
    /// </summary>
    public void ResetComboData()
    {
        ResuableDataAttack.comboCount = 0;
        ResuableDataAttack.canInput = true;
        ResuableDataAttack.canMoveInterrupt = true;
        ReverseATK();
    }

    public void AttackAni_EnterSet()
    {
        //ReverseATK();
    }

    public void AttackAni_UpdateSet()
    {
        if (!notMoveInput())
        {
            stateMachine.State = StateAction.walk;
            player.SetBoolServerRpc(aniHarsh.HasInputID, true);
            SoundClear();
        }
    }

    #region 检查敌人

    public float curDistance;

    /// <summary>
    /// 用来计算两点之间的距离
    /// </summary>
    /// <param name="obj1"></param>
    /// <param name="obj2"></param>
    /// <returns></returns>
    public Vector2 HorizontalDistance(Transform obj1, Transform obj2)
    {
        Vector2 posxz = new Vector2(
            obj1.transform.position.x - obj2.transform.position.x, 
            obj1.transform.position.z - obj2.transform.position.z);
        
        return posxz;
    }

    /// <summary>
    /// 维护最近的敌人，查看是否还有效（因为距离是动态变化）
    /// </summary>
    /// <returns></returns>
    public GameObject CheckEnemyIsValid()
    {
        if (ResuableDataAttack.Enemy_MinDistance ==null) return null;
        Vector2 posxz = HorizontalDistance(ResuableDataAttack.Enemy_MinDistance.transform, player.transform);
        
        if (posxz.magnitude > content.AttackData.CheckDistance)
        {
            ResuableDataAttack.Enemy_MinDistance = null;
            curDistance = Mathf.Infinity;
            return null;
        }

        return ResuableDataAttack.Enemy_MinDistance;
    }

    /// <summary>
    /// 实时查看是否有新的满足距离的enemy，更新最近的enemy
    /// </summary>
    public void CheckEnemyInDistance()
    {
        Collider[] colliders = Physics.OverlapSphere(
            player.transform.position, content.AttackData.CheckDistance, 
            content.AttackData.enemyMask, QueryTriggerInteraction.Ignore);
        
        if (colliders.Length == 0) return;
        
        foreach (var collider in colliders)
        {
            Vector2 posxz = HorizontalDistance(collider.transform, player.transform);
            
            if (ResuableDataAttack.Enemy_MinDistance == null || posxz.magnitude < curDistance)
            {
                ResuableDataAttack.Enemy_MinDistance = collider.gameObject;
                curDistance = posxz.magnitude;
                //Debug.Log(collider.gameObject.name);
            }
        }
    }
    
    #endregion
   
}
