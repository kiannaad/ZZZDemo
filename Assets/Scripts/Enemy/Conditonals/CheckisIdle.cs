using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckisIdle : Conditional
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Failure;
    }
}
