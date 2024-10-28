using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Singleton<Counter>
{
    [SerializeField] private Transform firstPos;
    [SerializeField] private Transform paperBag;
    public Transform GetPaperBagPos() {  return paperBag; }
    [SerializeField] private float lineGap;
    [SerializeField] private MoneyManager moneyManager;


    private Queue<Customer> customers = new Queue<Customer>();
    private Queue<Transform> LinePos = new Queue<Transform>();

    public bool isPayable = false;

    private bool isPacking = false;
    public bool IsPacking { get { return isPacking; } }

    public void EnqueueCustomer(Customer customer)
    {
        customers.Enqueue(customer);
    }
    public void Pay(Customer customer)
    {
        int price = customer.GetPriceToPay();
        moneyManager.InstanceMoney(price);
        isPacking = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            isPayable = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            isPayable = false;
        }
    }

    private void Update()
    {
        if (isPayable)
        {
            Queueing();
        }
    }


    public Vector3 PaperBagPos() { return paperBag.position; }
    public Vector3 GetPosToWait()
    {
        Vector3 pos = firstPos.position;
        pos.z += lineGap * customers.Count;

        return pos;
    }
    public void RePosCustomers()
    {
        foreach (var customer in customers)
        {
            var dest = customer.transform.position;
            dest.z -= lineGap;
            customer.AINavMoveToward(dest);
        }
    }

    public void Queueing()
    {
        if (!isPacking && customers.Count > 0)
        {
            var customer = customers.Dequeue();
            customer.isPacking = true;
            isPacking = true;
        }
    }
}
