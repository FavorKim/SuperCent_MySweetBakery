using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro.EditorUtilities;
using UnityEngine;

public class ObjectPool <T>  where T : MonoBehaviour
{
    private T prefab;
    private int poolCount;

    private Queue<T> pool = new Queue<T>();
    private Transform parent;

    public ObjectPool(T prefab , int poolCount, Transform parent)
    {
        this.prefab = prefab;
        this.poolCount = poolCount;
        var poolParent = new GameObject(prefab.name + "Pool");
        poolParent.transform.SetParent(parent);
        this.parent = poolParent.transform;
        InitPool();
    }


    private void InitPool()
    {
        if (prefab == null) return;
        
        for(int i = 0; i < poolCount; i++)
        {
            T go = GameObject.Instantiate(prefab, parent);
            
            go.gameObject.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public void EnqueueObject(T go)
    {
        pool.Enqueue(go);
        go.transform.SetParent(parent);
        go.gameObject.SetActive(false);
    }

    public T GetObject()
    {
        T obj;
        if (pool.Count == 0)
        {
            var create = GameObject.Instantiate(prefab);
            EnqueueObject(create);
        }

        obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ClearPool()
    {

        if(pool.Count == 0) return;

        foreach (var go in pool)
        {
            GameObject.Destroy(go);
        }

        pool.Clear();
    }

}
