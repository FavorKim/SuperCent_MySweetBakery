using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPoolManager : Singleton<MoneyPoolManager>
{
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private int poolCount = 100;

    private ObjectPool moneyPool;

    

    private void InitPool()
    {
        moneyPool = new ObjectPool(moneyPrefab, poolCount, transform);
    }

    public GameObject GetMoney()
    {
        return moneyPool.GetObject();
    }

    public void ReturnMoney(GameObject money)
    {
        moneyPool.EnqueueObject(money);
    }
    protected override void OnAwake()
    {
        InitPool();
    }
}
