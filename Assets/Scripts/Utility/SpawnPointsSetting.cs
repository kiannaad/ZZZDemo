using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointsSetting : MonoBehaviour
{
    public List<Vector3> SpawnPoints = new List<Vector3>();
    private int index = 0;

    public NetworkObject player;
    public NetworkObject enemy;


    public Vector3 GetPoint()
    {
        if (index < SpawnPoints.Count)
        {
            var back = index;
            index = (index + 1) % SpawnPoints.Count;
            return SpawnPoints[index];
        }
        return Vector3.zero;
    }
    
}
