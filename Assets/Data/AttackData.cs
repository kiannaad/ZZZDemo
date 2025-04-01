using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackData
{
   public float attackRotationTime;
   public List<ComboData> LightCombos = new List<ComboData>();
   public ComboData SkillCombo;
   public ComboData FinishSKillCombo;

   public LayerMask enemyMask;
   [field : SerializeField ] public float CheckDistance { get; private set; }
}
