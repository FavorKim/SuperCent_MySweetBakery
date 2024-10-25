using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Singleton<Counter>
{
    [SerializeField] private Transform firstPos;
    private Queue<Customer> customers = new Queue<Customer>();
    [SerializeField] private float lineGap;
    public bool isPayable = false;
    [SerializeField] private Transform paperBag;

    [SerializeField] private Vector3 Gap;
    [SerializeField] private Transform moneyFirstPos;
    
    private Stack<GameObject> moneyStack = new Stack<GameObject>();

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

    public void Pay()
    {
        Customer customer = customers.Dequeue();
        int price = customer.GetPriceToPay();
        InstanceMoney(price);
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

    public void InstanceMoney(int price)
    {
        for(int i = 0; i < price; i++)
        {
            var money = MoneyPoolManager.Instance.GetMoney();
            money.transform.SetParent(moneyFirstPos.parent);

            int xMulti = moneyStack.Count % 3;
            int yMulti = moneyStack.Count / 9;
            int zMulti = moneyStack.Count / 3;
            
            moneyStack.Push(money);
            Vector3 pos = new Vector3(xMulti * Gap.x, yMulti * Gap.y, zMulti * Gap.z);
            pos += moneyFirstPos.localPosition;

            money.transform.localPosition = pos;
        }
    }
}
