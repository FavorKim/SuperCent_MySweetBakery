using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBagPoolManager : Singleton<PaperBagPoolManager>
{
    [SerializeField] private PaperBag PaperBagPrefab;

    private ObjectPool<PaperBag> pool;

    [SerializeField] private int poolCount = 5;

    private void InitPool()
    {
        pool = new ObjectPool<PaperBag>(PaperBagPrefab, poolCount, transform);
    }

    public PaperBag GetPaperBag()
    {
        if (pool == null) InitPool();

        return pool.GetObject();
    }

    public void ReturnPaperBag(PaperBag paperBag)
    {
        if (pool == null) InitPool();
        pool.EnqueueObject(paperBag);
    }
}
