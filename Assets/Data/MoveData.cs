using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveData 
{
    public float walkSpeed;
    public float runSpeed;
    public float moveTime;
    
    public float BufferTime_MoveToIdle;
    public float BufferTime_MoveToAttacking;
}
