using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Evolution later
/// </summary>
public class MyObjectPool : MonoBehaviour
{

    [System.Serializable]
    public class PrefabPool
    {

        public Transform prefab;
        internal GameObject prefabGO;
        internal MyObjectPool myObjPool;
        public int amount;
        public bool growUp;

        public List<Transform> spawned;
        public List<Transform> despawned;

        // Quản lý các event cho các item:
        public Dictionary<Transform, _OnDespawned> OnDespawnEventItem;

        #region Base Event - Các event này xài cho nguyên 1 object pool
        // Ví dụ: Khi enemy chết sẽ hiện ra 1 cục đạn.        

        public delegate void _OnCreatePrefabPoolItem(GameObject obj);
        public delegate void _OnDespawned();
        public delegate void _OnSpawned();

        public event _OnCreatePrefabPoolItem OnCreatePrefabPoolItem;
        public event _OnDespawned OnDespawned;
        public event _OnSpawned OnSpawned;

        #endregion



        public void CreateBaseData()
        {
            prefabGO = prefab.gameObject;
            spawned = new List<Transform>();
            despawned = new List<Transform>();
            OnDespawnEventItem = new Dictionary<Transform, _OnDespawned>();
        }

        public void CreatePrefabPool(Transform parent)
        {
            //    prefabGO.SetActive(false);
            for (int i = 0; i < amount; i++)
            {
                var obj = Instantiate(prefabGO);
                obj.transform.parent = parent;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(false);

                // Dành cho class lớn:
                if (OnCreatePrefabPoolItem != null)
                    OnCreatePrefabPoolItem(obj);
                // DÀnh cho các item oject nhỏ:               
                despawned.Add(obj.transform);

            }
        }

        public Transform SpawnInstance(Vector3 pos, Quaternion rot,bool callback = false)
        {
            Transform inst;

            if (despawned.Count == 0 && growUp) // create new one
            {
                var obj = Instantiate(prefabGO);
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.transform.parent = myObjPool.group;
                spawned.Add(obj.transform);
                obj.SetActive(true);

                inst = obj.transform;


            }
            else
            {
                inst = despawned[0];
                despawned.RemoveAt(0);
                spawned.Add(inst);

                inst.position = pos;
                inst.rotation = rot;
                inst.gameObject.SetActive(true);
            }

            if (OnSpawned != null)
                OnSpawned();

            if (callback == true)
                inst.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);

            return inst;

        }

        public bool DespawnInstance(Transform transform, bool callback = false)
        {
            spawned.Remove(transform);
            despawned.Add(transform);
            transform.gameObject.SetActive(false);

            if (OnDespawned != null)
                OnDespawned();

            if (callback == true)
                transform.gameObject.BroadcastMessage("OnDespawned", this, SendMessageOptions.DontRequireReceiver);

            return true;
        }

    }

    public string poolName;
    public List<PrefabPool> prefabPoolList = new List<PrefabPool>();
    private Transform group;


    private void Awake()
    {
        group = this.transform;

        for (int i = 0; i < prefabPoolList.Count; i++)
        {
            if (prefabPoolList[i].prefab == null)
            {
                Debug.LogError("Missing data");
                continue;
            }
            this.prefabPoolList[i].CreateBaseData();
            prefabPoolList[i].myObjPool = this;
            prefabPoolList[i].CreatePrefabPool(group);
        }

        PoolManager.Instance.myObjectPools.Add(poolName, this);
    }

    public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent, bool callback = false, PrefabPool._OnDespawned _OnDespawnedItem = null)
    {
        Transform instance;

        for (int i = 0; i < prefabPoolList.Count; i++)
        {
            if (prefabPoolList[i].prefabGO == prefab.gameObject)
            {
                instance = prefabPoolList[i].SpawnInstance(pos, rot, callback);

                if (_OnDespawnedItem != null)
                {
                    // Check if instance exist in dictionary:
                    if(prefabPoolList[i].OnDespawnEventItem.ContainsKey(instance))
                        prefabPoolList[i].OnDespawnEventItem[instance] = _OnDespawnedItem;
                    else
                        prefabPoolList[i].OnDespawnEventItem.Add(instance, _OnDespawnedItem);
                }
                    

                if (instance == null)
                    return null;

                if (parent != null)
                    instance.transform.parent = parent;

                return instance;
            }
        }

        return null;
    }


    public bool Despawn(Transform instance,bool callback = false)
    {
        bool despawned = false;
        for (int i = 0; i < prefabPoolList.Count; i++)
        {
            if (prefabPoolList[i].spawned.Contains(instance))
            {
                despawned = prefabPoolList[i].DespawnInstance(instance,callback);

               if(prefabPoolList[i].OnDespawnEventItem.ContainsKey(instance))
                    prefabPoolList[i].OnDespawnEventItem[instance]();
                    
                break;
            }
        }

        if (!despawned)
        {
            //   Debug.LogError("Prefab pool not found");
            return false;
        }
        return true;
    }

    public void Despawn(Transform instance, Transform parent, bool callback = false)
    {
        instance.parent = parent;
        Despawn(instance,callback);
    }

}
