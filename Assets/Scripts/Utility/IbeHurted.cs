using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IbeHurted 
{
   public void OnHurted(Vector2 direction, float damage);
   public void OnKilled(Vector2 direction);
   public bool CheckOnDied(float damage);
}
