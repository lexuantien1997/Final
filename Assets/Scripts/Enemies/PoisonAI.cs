using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAI : MonoBehaviour {

    EnemyAI enemyAI;
   
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemyAI.TargetVisible == null)
            enemyAI.ScanTargetInLongAttackArea();
        else
        {
            enemyAI.CheckTargetStillVisible_InLongArea();
           // enemyAI.Face2Target();
            enemyAI.LookAtTarget();
            enemyAI.CheckShootTime();            
            enemyAI.RememberTargetPos();
         //   enemyAI.Face2Face();
        }
    }

    public void Despawned()
    {
       var existInPool = PoolManager.Instance.myObjectPools["Enemies"].Despawn(transform);
       if (!existInPool)
            Destroy(gameObject);
    }
}
