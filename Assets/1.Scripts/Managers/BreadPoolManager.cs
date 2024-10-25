using System.Collections.Generic;
using UnityEngine;

public class BreadPoolManager : Singleton<BreadPoolManager>
{
    [SerializeField] private List<Bread> breadPrefabs = new List<Bread>();

    [SerializeField] private int poolCount = 20;

    Dictionary<BreadType, ObjectPool<Bread>> breadPools = new Dictionary<BreadType, ObjectPool<Bread>>();
    

    private void InitBreadPools()
    {
        foreach (var bread in breadPrefabs)
        {
            ObjectPool<Bread> pool = new ObjectPool<Bread>(bread, poolCount, transform);
            breadPools.Add(bread.breadName, pool);
        }
    }

    public Bread GetBread(BreadType name)
    {
        Bread bread = breadPools[name].GetObject();

        return bread;
    }

    public void ReturnBread(Bread bread)
    {
        breadPools[bread.breadName].EnqueueObject(bread);
    }

    protected override void OnAwake()
    {
        InitBreadPools();
    }
}
