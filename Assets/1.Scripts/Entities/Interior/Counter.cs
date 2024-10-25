using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Singleton<Counter>
{
    [SerializeField] private Transform firstPos;
    private Queue<Customer> customers = new Queue<Customer>();
    [SerializeField] private float lineGap;
    [SerializeField]private bool isPayable = false;
    [SerializeField] private Transform paperBag;
    

    bool isPacking = false;

    public void EnqueueCustomer(Customer customer)
    {
        customers.Enqueue(customer);
    }

    public Vector3 GetPosToWait()
    {
        Vector3 pos = firstPos.position;
        pos.z -= lineGap * customers.Count;

        return pos;
    }

    void Pay()
    {
        Customer customer = customers.Dequeue();
        customer.IsReadyToPay = true;

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
}
