using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    public SoundItem curAttackSound;
    
    /// <summary>
    /// 连击采用的数据应用封装
    /// </summary>
    public void ATK()
    {
        ComboData data = ResuableDataAttack.comboData[ResuableDataAttack.comboCount];
        string name = data.comboName;
        UpdateDamageInfo(player.transform.forward, data.damage, data.ShakeForce);
        animator.CrossFade(name, 0.111f);

        curAttackSound = AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.sweaponSound);
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.CharacterSounds);
        
        ATKSet();
    }

    /// <summary>
    /// 小技能采用的数据应用封装
    /// </summary>
    public void SkillATK()
    {
        ComboData data = ResuableDataAttack.skillData;
        string name = data.comboName;
        UpdateDamageInfo(player.transform.forward, data.damage, data.ShakeForce);
        animator.CrossFade(name, 0.111f);

        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.sweaponSound);
        AudioClipPoolManager.Instance.PlayAudioClip(player.poolType, data.CharacterSounds);
       
    }

    /// <summary>
    /// 大招采用的数据应用封装
    /// </summary>
    public void FinishSkillATK()
    {
        ComboData data = ResuableDataAttack.finishSkillData;
        string name = data.comboName;
        UpdateDamageInfo(player.transform.forward, data.damage, data.ShakeForce);
        animator.CrossFade(name, 0.111f);
        
        //AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.sweaponSound);
        //AudioClipPoolManager.Instance.PlayAudioClip(PoolType.AnBi_AudioPool, data.CharacterSounds);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ATKSet()
    {
        ResuableDataAttack.canInput = false;
        ResuableDataAttack.canMoveInterrupt = false;
        AddComboCount();
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
    }

    public void AttackAni_EnterSet()
    {
        ResetComboData();
    }

    public void AttackAni_UpdateSet()
    {
        if (!notMoveInput())
        {
            stateMachine.State = StateAction.walk;
            animator.SetBool(aniHarsh.HasInputID, true);
            curAttackSound?.gameObject.SetActive(false);
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

    /// <summary>
    /// 更新伤害数据info
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="damage"></param>
    /// <param name="shakeForce"></param>
    private void UpdateDamageInfo(Vector2 direction, float damage, float shakeForce)
    {
        ResuableDataAttack.damageInfo.damage = damage;
        ResuableDataAttack.damageInfo.direction = direction;
        ResuableDataAttack.damageInfo.shakeForce = shakeForce;
    }
}
