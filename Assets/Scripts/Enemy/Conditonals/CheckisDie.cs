using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckisDie : Conditional
{
    public SharedBool isDie;

    public override TaskStatus OnUpdate()
    {
        if (isDie.Value) return TaskStatus.Success;
        return TaskStatus.Failure;
    }
}
