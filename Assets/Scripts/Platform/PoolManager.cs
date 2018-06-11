using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class PoolManager {

    static PoolManager instance;

    public static PoolManager Instance
    {
        get
        {
            if (instance != null)
                return instance;
            else
            {
                instance = new PoolManager();
                return instance;
            }
        }       
    }

    public PoolManager()
    {
        myObjectPools = new Dictionary<string, MyObjectPool>();
    }

    public Dictionary<string, MyObjectPool> myObjectPools;



}
