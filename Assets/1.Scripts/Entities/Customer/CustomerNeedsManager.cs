using System.Collections.Generic;

public class CustomerNeedsManager
{
    private Queue<CustomerNeeds> needsQueue = new Queue<CustomerNeeds>();
    private CustomerNeeds currentNeeds;
    
    public void EnqueueNeeds(CustomerNeeds customerNeeds)
    {
        if (needsQueue.Count == 0)
            currentNeeds = customerNeeds;

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
        currentNeeds = needsQueue.Dequeue();
        currentNeeds.OnEnter();
    }
}
