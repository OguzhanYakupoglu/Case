using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size = 10;
        public Dictionary<GameObject, bool> pooledObjects = new();
    }

    public List<Pool> pools;

    protected override void Awake()
    {
        base.Awake();

        foreach (var pool in pools)
        {
            SpawnObjects(pool, pool.size);
        }
    }

    private void SpawnObjects(Pool pool, int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnNewObject(pool);
        }
    }

    private GameObject SpawnNewObject(Pool pool)
    {
        var newObject = Instantiate(pool.prefab, transform);
        newObject.SetActive(false);
        pool.pooledObjects.Add(newObject, false);
        return newObject;
    }

    public GameObject Get(string tag)
    {
        var pool = pools.FirstOrDefault(p => p.tag == tag);
        if (pool == null)
        {
            Debug.LogWarning($"ObjectPooler tag '{tag}' not found");
            return null;
        }

        var availableObject = pool.pooledObjects.FirstOrDefault(po => !po.Value);
        if (availableObject.Key != null)
        {
            pool.pooledObjects[availableObject.Key] = true;
            ActivateObject(availableObject.Key);
            return availableObject.Key;
        }

        var newObj = SpawnNewObject(pool);
        pool.pooledObjects[newObj] = true;
        ActivateObject(newObj);
        return newObj;
    }

    private void ActivateObject(GameObject obj)
    {
        obj.SetActive(true);
        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnGet();
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        foreach (var pool in pools)
        {
            if (pool.pooledObjects.ContainsKey(obj))
            {
                pool.pooledObjects[obj] = false;
                if (obj.TryGetComponent<IPoolable>(out var poolable))
                {
                    poolable.OnReturn();
                }
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                return;
            }
        }
    }

    public void ReturnAllPools()
    {
        foreach (var pool in pools)
        {
            var activeObjects = pool.pooledObjects.Where(po => po.Value).ToList();

            foreach (var po in activeObjects)
            {
                ReturnToPool(po.Key.gameObject);
            }
        }
    }
    
}
