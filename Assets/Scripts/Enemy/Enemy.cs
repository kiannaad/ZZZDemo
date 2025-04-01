using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHurted
{
    public void beAttacked(Vector2 direction, float damage)
    {
       // Debug.Log($"Direction: {direction}, Damage: {damage}");
    }
}
