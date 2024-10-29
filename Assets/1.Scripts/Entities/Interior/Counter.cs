using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Counter : Singleton<Counter>
{
    [SerializeField] private Transform paperBag;
    public Transform GetPaperBagPos() {  return paperBag; }
    [SerializeField] private MoneyManager moneyManager;

    public CustomerLine packingLine;
    public CustomerLine tableLine;

    private UnityEvent OnTutorialClear = new UnityEvent();

    public bool isPayable = false;

    protected override void OnAwake()
    {
        TutorialArrowController.Instance.AddCondition(OnTutorialClear, 2);
    }


    public void Packing_Pay(Customer customer)
    {
        int price = customer.GetPriceToPay();
        moneyManager.InstanceMoney(price);
        if (TutorialArrowController.Instance.CurrentTutorialLevel == 3)
            OnTutorialClear.Invoke();
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



    public Vector3 PaperBagPos() { return paperBag.position; }

}
