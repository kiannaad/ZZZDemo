using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckInCloseDistance : Conditional
{
    public SharedFloat distance;
    public float minDistance;
    public float maxDistance;

    public override TaskStatus OnUpdate()
    {
        if (distance.Value >= minDistance && distance.Value <= maxDistance)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
