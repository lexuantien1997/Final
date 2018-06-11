using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndrasAI : MonoBehaviour {

    public float timeChangeDirectionPatrolling;
    public EnemyAI enemyAI;

    // Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    //void FixedUpdate () {
    //       // enemyAI.SamplePatroll();
    //}

    public void Despawned()
    {
        var existInPool = PoolManager.Instance.myObjectPools["Enemies"].Despawn(transform);
        if (!existInPool)
            Destroy(gameObject);
    }

}
