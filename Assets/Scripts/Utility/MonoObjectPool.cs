using System.Collections.Generic;
using UnityEngine;

public class MonoObjectPool<T> where T : MonoBehaviour, IPoolable
{
    private GameObject poolParent;
    
    private readonly List<T> pool = new List<T>();
    private readonly T prefab;

    public MonoObjectPool(T prefab)
    {
        this.prefab = prefab;
    }

    public void PoolAll()
    {
        foreach (var poolable in pool)
        {
            PoolObject(poolable);
        }
    }

    public void PoolObject(T objectToPool) // can pool invalid object that does not exist in pool. 
    {
        objectToPool.gameObject.SetActive(false);
        objectToPool.WhenPooled();
    }

    public T Spawn(Vector2 position, Quaternion rotation, Transform parent)
    {
        return GetNextOrExpand(position, rotation, parent);
    }

    private T GetNextOrExpand(Vector2 position, Quaternion rotation, Transform parent)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                var inactiveObject = pool[i];
                inactiveObject.Init();
                return inactiveObject;
            }
        }

        var newPoolable = GameObject.Instantiate(prefab, position, rotation, parent);
        newPoolable.Init();
        pool.Add(newPoolable);
        
        return newPoolable;
    }
}