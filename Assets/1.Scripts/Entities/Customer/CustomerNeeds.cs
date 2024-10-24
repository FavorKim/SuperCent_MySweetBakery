
public interface CustomerNeeds
{
    void OnEnter();
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

    }
    public bool EvaluateCompleteCondition()
    {
        return customer.breadCount == 0;
    }

    public void OnComplete()
    {

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

    }
    public bool EvaluateCompleteCondition()
    {
        return customer.isReadyToPay;
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

    public void OnComplete()
    {

    }
}