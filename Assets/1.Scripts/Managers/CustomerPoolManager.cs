using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPoolManager : Singleton<CustomerPoolManager>
{
    [SerializeField] private Customer customerPrefab;

    [SerializeField] private int poolCount = 10;

    private ObjectPool<Customer> pool;


    private void InitPool()
    {
        pool = new ObjectPool<Customer>(customerPrefab, poolCount, transform);
    }

    public Customer GetCustomer()
    {
        if (pool == null) InitPool();

        return pool.GetObject();
    }

    public void ReturnCustomer(Customer customer)
    {
        if (pool == null) InitPool();
        pool.EnqueueObject(customer);
    }
}
