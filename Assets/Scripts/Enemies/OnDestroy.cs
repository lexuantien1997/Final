using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroy : MonoBehaviour {

    public float time;
    public string poolName;
    public float startTime;


	// Use this for initialization
	void Start () {
        startTime = 0;
    }

    private void OnEnable()
    {
        startTime = 0;
    }

	// Update is called once per frame
	void Update () {

		if(time > 0)
        {
            startTime += Time.deltaTime;            
            if(startTime > time)
            {
                PoolManager.Instance.myObjectPools[poolName].Despawn(transform);
                startTime = -1;
            }
        }
	}

    public void Despawn()
    {
        PoolManager.Instance.myObjectPools[poolName].Despawn(transform);
    }
}
