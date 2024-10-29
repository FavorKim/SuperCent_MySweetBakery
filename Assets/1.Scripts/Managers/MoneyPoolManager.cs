using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPoolManager : Singleton<MoneyPoolManager>
{
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private int poolCount = 100;

    private ObjectPool moneyPool;

    private int entireMoneySpawned = 0;
    private int EntireMoneySpawned
    {
        get { return entireMoneySpawned; }
        set
        {
            if (entireMoneySpawned != value)
            {
                entireMoneySpawned = value;
            }
            if (entireMoneySpawned >= 30)
            {
                GameManager.Instance.PointCamUnlockActive(1);
            }
        }
    }

    private void InitPool()
    {
        moneyPool = new ObjectPool(moneyPrefab, poolCount, transform);
    }

    public GameObject GetMoney()
    {
        EntireMoneySpawned = entireMoneySpawned + 1;
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
