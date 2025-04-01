using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct damageInfo
{
    public Vector2 direction;
    public float damage;
    public float shakeForce;
}

public class GameManager : MonoSigleton<GameManager>
{
    public void HurtProcess(damageInfo Info, GameObject defender)
    {
        defender.GetComponent<IHurted>().beAttacked(Info.direction, Info.damage);
    }
}
