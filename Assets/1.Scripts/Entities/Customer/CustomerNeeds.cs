using UnityEngine;

public interface CustomerNeeds
{
    void OnEnter();
    void OnReached();
    bool EvaluateCompleteCondition();
    void OnComplete();
}

public class NeedBread : CustomerNeeds
{
    private Customer customer;
    public NeedBread(Customer customer)
    {
        this.customer = customer;
    }

    public void OnEnter()
    {
        customer.SetBreadCountRandomly();

        Vector3 destination = AINavigator.Instance.GetSaleShelvesWaitingPos();
        customer.AINavMoveToward(destination);
    }
    public bool EvaluateCompleteCondition()
    {
        return customer.BreadCountToNeed == 0;
    }
    public void OnReached()
    {
        Vector3 shelvPos = AINavigator.Instance.GetSaleShelvesPos();
        customer.RotateToward(shelvPos);
        customer.OnReached_Bread();
        
    }
    public void OnComplete()
    {
        customer.OnComplete_Bread();
    }
}

public class NeedPay : CustomerNeeds
{
    private Customer customer;
    public NeedPay(Customer customer)
    {
        this.customer = customer;
    }

    public void OnEnter()
    {
        customer.OnEnter_Pay();
    }
    public bool EvaluateCompleteCondition()
    {
        return customer.IsReadyToPay;
    }
    public void OnReached()
    {

    }

    public void OnComplete()
    {

    }
}

public class NeedTable : CustomerNeeds
{
    private Customer customer;
    public NeedTable(Customer customer)
    {
        this.customer = customer;
    }

    public void OnEnter()
    {

    }
    public bool EvaluateCompleteCondition()
    {
        return customer.isTableAvailable;
    }

    public void OnReached()
    {

    }
    public void OnComplete()
    {

    }
}