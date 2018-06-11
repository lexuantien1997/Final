using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour {

    public string poolName;
    public int totalNumber;
    public int concurent2BeSpawned;
    public Transform objSpawn;
    public float spawnDelay;
    public float spawnArea;

    protected int totalSpawned,totalCurrentEnemySpawn,realTotalNumber;
    protected WaitForSeconds spawnWait;
    protected Coroutine spawnTimerCoroutine;


    private void OnDisable()
    {
        //var e = PoolManager.Instance.myObjectPools[poolName].prefabPoolList.Find(x => x.prefab == objSpawn.transform);
        //e.OnDespawned -= E_OnDespawned;

       


    }
    

    // Use this for initialization
    void Start () {

        for (int i = 0; i < concurent2BeSpawned; i++)
        {          
            PoolManager.Instance.myObjectPools[poolName].Spawn(objSpawn, transform.position + transform.right * Random.Range(-spawnArea * 0.5f, spawnArea * 0.5f), Quaternion.identity,null,false,E_OnDespawned);

        }


        realTotalNumber = totalNumber < concurent2BeSpawned ? concurent2BeSpawned : totalNumber;
        totalSpawned = concurent2BeSpawned;
        totalCurrentEnemySpawn = concurent2BeSpawned;
        spawnWait = new WaitForSeconds(spawnDelay);
    }

    private void E_OnDespawned()
    {
        totalCurrentEnemySpawn--;
        Debug.Log(totalCurrentEnemySpawn);
     
        if (spawnTimerCoroutine == null)
            spawnTimerCoroutine = StartCoroutine(SpawnTimer());
    }

    protected IEnumerator SpawnTimer()
    {
        while (totalCurrentEnemySpawn < concurent2BeSpawned && totalSpawned < realTotalNumber)
        {
            yield return spawnWait;
            PoolManager.Instance.myObjectPools[poolName].Spawn(objSpawn, transform.position + transform.right * Random.Range(-spawnArea * 0.5f, spawnArea * 0.5f), Quaternion.identity, null,false, E_OnDespawned);
            totalCurrentEnemySpawn++;
            totalSpawned++;
        }

        spawnTimerCoroutine = null;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea, 0.4f, 0));
    }
}
