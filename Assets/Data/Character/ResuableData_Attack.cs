using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResuableData_Attack
{
   public ResuableData_Attack()
   {
      comboCount = 0;
   }
   public int comboCount;
   public List<ComboData> comboData;
   public ComboData skillData;
   public bool canMoveInterrupt;
   public bool canInput;
   public bool canRotate;
}
