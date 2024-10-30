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

    [SerializeField] private Animator footStep;

    private bool isPayable = false;
    public bool IsPayable
    {
        get { return isPayable; }
        set
        {
            if (isPayable != value)
            {
                isPayable = value;
                footStep.SetBool("isPayable", isPayable);
            }
        }
    }

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
            IsPayable = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            IsPayable = false;
        }
    }



    public Vector3 PaperBagPos() { return paperBag.position; }

}
