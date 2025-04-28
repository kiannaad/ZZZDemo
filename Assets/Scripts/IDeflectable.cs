using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeflectable 
{
    public Action OnDeflected{get;set;}
    public bool CanbeDeflected { get; set; }
    public void EnableDeflection() => CanbeDeflected = true;
    public void DisableDeflection() => CanbeDeflected = false;
    
    public bool CompledtedDeflection { get; set; }
    public void EnableCompledtedDeflection() => CompledtedDeflection = true;
    public void DisableCompledtedDeflection() => CompledtedDeflection = false;
}
