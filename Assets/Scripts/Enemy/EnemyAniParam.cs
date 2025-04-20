using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAniParam : MonoBehaviour
{
   private Enemy _enemy;

   private void Start()
   {
      _enemy = GetComponent<Enemy>();
   }

   public void EnableBehaviourTree() => _enemy.behaviorTree.enabled = true;
   public void DisableSelf() => Destroy(gameObject);

   public void PlayEnemyBlock8Audio() =>
      AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Enemy, AudioClipType.Blocks8);

   public void PlayEnemyGutKick8Audio() =>
      AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Enemy, AudioClipType.gutkick8);

   public void PlayEnemykickshollywood8Audio() =>
      AudioClipPoolManager.Instance.PlayAudioClip(Character_Name.Enemy, AudioClipType.kickshollywood8);

   public void ForceToFace() => _enemy.canAttackRotate = true;
   public void CanRunRotate() => _enemy.canRunAgent = true;

   public void AgentInit()
   {
      GetComponent<Animator>().ResetTrigger("stopRun");
      _enemy.behaviorTree.enabled = false;
      _enemy.CanChase = true;
      _enemy.canRunAgent = true;
   }

   public void AgentStop()
   {
      Debug.Log("AgentStop");
      _enemy.ResetAgent();
      _enemy.CanChase = false;
      _enemy.canRunAgent = false;
   }
}
