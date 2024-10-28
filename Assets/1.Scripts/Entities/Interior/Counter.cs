using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Singleton<Counter>
{
    [SerializeField] private Transform paperBag;
    public Transform GetPaperBagPos() {  return paperBag; }
    [SerializeField] private MoneyManager moneyManager;

    public CustomerLine packingLine;
    public CustomerLine tableLine;


    public bool isPayable = false;


    
    public void Packing_Pay(Customer customer)
    {
        int price = customer.GetPriceToPay();
        moneyManager.InstanceMoney(price);
        packingLine.IsStop = false;
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
            packingLine.Queueing();
            tableLine.Queueing();
        }
    }


    public Vector3 PaperBagPos() { return paperBag.position; }

}
