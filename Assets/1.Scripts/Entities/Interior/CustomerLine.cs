using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerLine : MonoBehaviour
{
    [SerializeField] private Transform firstPos;
    private float lineGap = 1.24f;
    private Queue<Customer> customers = new Queue<Customer>();

    public bool IsStop { get; set; }


    public void EnqueueCustomer(Customer customer)
    {
        customers.Enqueue(customer);
    }

    public Vector3 GetPosToWait()
    {
        Vector3 pos = firstPos.position;
        pos.z += lineGap * customers.Count;

        return pos;
    }
    private Vector3 GetPosToWait(int index)
    {
        Vector3 pos = firstPos.position;
        pos.z += lineGap * index;

        return pos;
    }
    public void RePosCustomers()
    {
        int count = 0;
        foreach (var customer in customers)
        {
            var dest = GetPosToWait(count);
            dest.z -= lineGap;
            customer.AINavMoveToward(dest);
            count++;
        }
    }

    public void Queueing()
    {
        if (customers.Count > 0 && !IsStop)
        {
            var customer = customers.Dequeue();
            customer.isPacking = true;
            IsStop = true;
        }
    }
}
