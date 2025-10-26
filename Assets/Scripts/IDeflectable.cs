using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IDeflectable 
{
    public Action OnDeflected{get;set;}
    public NetworkVariable<bool> CanbeDeflected { get; set; }
    public void EnableDeflection();
    public void DisableDeflection();
    
    public NetworkVariable<bool> CompledtedDeflection { get; set; }
    public void EnableCompledtedDeflection();
    public void DisableCompledtedDeflection();
}
