
using System.Collections.Generic;
using UnityEngine;

public class FlyingKnife
{
    private float dectedDistance;
    private Transform player;
    private LayerMask enemyMask;
    private List<GameObject> EnemyObj;

    public FlyingKnife(float dectedDistance, LayerMask enemyMask, Transform player)
    {
        EnemyObj = new List<GameObject>();
        this.dectedDistance = dectedDistance;
        this.enemyMask = enemyMask;
        this.player = player;
    }
    
    public GameObject TryGetCanFlyEnemy(Vector3 position)
    {
        if (EnemyObj.Count != 0) EnemyObj.Clear();
        
        var enemies = Physics.OverlapSphere(position, dectedDistance, enemyMask, QueryTriggerInteraction.Ignore);
        foreach (var enemy in enemies)
        {
            var Deflectable = enemy.gameObject.GetComponent<IDeflectable>().CanbeDeflected;
            if (Deflectable.Value && Vector2.Angle(new Vector2(player.transform.position.x - enemy.transform.position.x, player.transform.position.z - enemy.transform.position.z), 
                    new Vector2(enemy.transform.forward.x, enemy.transform.forward.z)) <= 85f) 
                EnemyObj.Add(enemy.gameObject);
        }
        
        if (EnemyObj.Count == 0) return null;
        else
        {
            return EnemyObj[0];
        }
    }

    public void FlyingAllEnemy()
    {
        if (EnemyObj.Count == 0) return;

        foreach (var enemy in EnemyObj)
        {
            enemy.GetComponent<IDeflectable>().EnableCompledtedDeflection();
        }
        
        EnemyObj.Clear();
    }
}
