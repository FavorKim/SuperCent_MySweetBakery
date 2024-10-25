using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Singleton<Counter>
{
    [SerializeField] private Transform firstPos;
    [SerializeField] private Transform paperBag;
    [SerializeField] private float lineGap;
    [SerializeField] private MoneyManager moneyManager;

    
    private Queue<Customer> customers = new Queue<Customer>();

    public bool isPayable = false;
    private bool isPacking = false;



    public void EnqueueCustomer(Customer customer)
    {
        customers.Enqueue(customer);
    }
    public void Pay()
    {
        Customer customer = customers.Dequeue();
        int price = customer.GetPriceToPay();
        moneyManager.InstanceMoney(price);
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


    public Vector3 PaperBagPos() { return  paperBag.position; }
    public Vector3 GetPosToWait()
    {
        Vector3 pos = firstPos.position;
        pos.z -= lineGap * customers.Count;

        return pos;
    }


}
