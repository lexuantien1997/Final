using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pharaoh_Snake : MonoBehaviour {


    EnemyAI enemyAI;

	// Use this for initialization
	void Start () {
        enemyAI = GetComponent<EnemyAI>();
	}
	
	// Update is called once per frame
	void Update () {
        if (enemyAI.CheckForObstacle(enemyAI.movement.speed))
            enemyAI.Flip();
        enemyAI.SetHorizontalSpeed(enemyAI.movement.speed);

    }

    public void Despawned()
    {
        var existInPool = PoolManager.Instance.myObjectPools["Enemies"].Despawn(transform);
        if (!existInPool)
            Destroy(gameObject);
    }

}
