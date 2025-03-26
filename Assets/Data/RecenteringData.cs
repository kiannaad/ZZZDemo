using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecenteringData
{
   [Range(0, 360)] public float minAngle;
   [Range(0, 360)] public float maxAngle;
   [Range(-1, 10)] public float waitTime;
   [Range(-1, 10)] public float recenterTime;
   
   public bool isinAngle(float angle) => angle >= minAngle && angle <= maxAngle;
}
