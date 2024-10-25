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

        customer.posToGo = DestinationManager.Instance.GetSaleShelvesWaitingPos();
        customer.AINavMoveToward(customer.posToGo.GetPosition());
    }
    public bool EvaluateCompleteCondition()
    {
        return customer.BreadCountToNeed == 0;
    }
    public void OnReached()
    {
        Vector3 shelvPos = DestinationManager.Instance.GetSaleShelvesPos();
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
        return Counter.Instance.isPayable && customer.IsReached;
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
        Counter.Instance.Pay();
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

public class GoBack : CustomerNeeds
{
    private Customer customer;
    public GoBack(Customer customer)
    {
        this.customer = customer;
    }

    public void OnEnter()
    {
        Vector3 entrance = DestinationManager.Instance.GetEntrancePos();
        customer.AINavMoveToward(entrance);
    }
    public bool EvaluateCompleteCondition()
    {
        return customer.IsReached;
    }

    public void OnReached()
    {

    }
    public void OnComplete()
    {
        customer.OnEndCustomerAI();
    }
}