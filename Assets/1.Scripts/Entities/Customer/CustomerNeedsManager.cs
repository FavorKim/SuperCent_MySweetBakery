using System.Collections.Generic;

public class CustomerNeedsManager
{
    private Queue<CustomerNeeds> needsQueue = new Queue<CustomerNeeds>();
    private CustomerNeeds currentNeeds;
    
    public void EnqueueNeeds(CustomerNeeds customerNeeds)
    {

        if (needsQueue.Count == 0)
        {
            currentNeeds = customerNeeds;
            currentNeeds.OnEnter();
        }
        needsQueue.Enqueue(customerNeeds);
    }


    public void RunNeedsQueue()
    {
        if (currentNeeds.EvaluateCompleteCondition() == true)
        {
            ProgressNeedsQueue();
        }
    }

    private void ProgressNeedsQueue()
    {
        currentNeeds.OnComplete();


        var nextNeeds = needsQueue.Dequeue();
        while(nextNeeds == currentNeeds)
        {
            nextNeeds = needsQueue.Dequeue();
        }
        currentNeeds = nextNeeds;
        currentNeeds.OnEnter();
    }

    public void ResetCustomerNeedsManager()
    {
        needsQueue.Clear();
        currentNeeds = null;
    }
    public void OnReached()
    {
        currentNeeds.OnReached();
    }
}
