using UnityEngine;
using static UnityEditor.PlayerSettings;

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
        customer.transform.rotation = Quaternion.LookRotation(shelvPos);

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
        Vector3 posToWait = Counter.Instance.GetPosToWait();
        customer.AINavMoveToward(posToWait);
        customer.transform.rotation = Quaternion.LookRotation(posToWait);

    }
    public bool EvaluateCompleteCondition()
    {
        return customer.IsReadyToPay;
    }
    public void OnReached()
    {
        customer.OnReached_Pay();
    }

    public void OnComplete()
    {

    }
}

public class NeedPacking : CustomerNeeds
{
    private Customer customer;
    public NeedPacking(Customer customer)
    {
        this.customer = customer;
    }

    public void OnEnter()
    {

    }
    public bool EvaluateCompleteCondition()
    {
        customer.OnPack();

        return customer.stackCount == 0;
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