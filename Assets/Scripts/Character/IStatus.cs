using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IStatus 
{
   public  NetworkVariable<float> health { get; set; }
   public float maxHealth { get; set; }
   public bool isDead { get; set; }
   public bool Invincible { get; set; }

   public void EnableInvincibility() => Invincible = true;
   public void DisableInvincibility() => Invincible = false;
   public void InvokeClientNoLocalHealthChange();
}
