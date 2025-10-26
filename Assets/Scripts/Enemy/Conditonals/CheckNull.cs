using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckNull : Conditional
{
    public SharedGameObject _object;
    
    public override TaskStatus OnUpdate()
    {
        if (_object.Value == null)
        {
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}
