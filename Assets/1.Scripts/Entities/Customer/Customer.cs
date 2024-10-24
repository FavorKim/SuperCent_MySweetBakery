using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    private NavMeshAgent agent;
    private CustomerNeedsManager needsManager;

    
    public bool IsReadyToPay { get; private set; }


    [SerializeField] private int breadCountRandomRange = 3;
    public int BreadCount { get; private set; }
    
    // 나중에 테이블 매니저가 나오면 교체할 가용 테이블이 있는지에 대한 변수
    public bool isTableAvailable = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        if (needsManager == null)
            needsManager = new CustomerNeedsManager();
        needsManager.EnqueueNeeds(new NeedBread(this));
        needsManager.EnqueueNeeds(new NeedPay(this));
    }
    private void OnDisable()
    {
        needsManager.ResetCustomerNeedsManager();
    }

    private void Update()
    {
        needsManager.RunNeedsQueue();
    }


    public void AINavMoveToward(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    public void SetBreadCountRandomly()
    {
        BreadCount = Random.Range(0, breadCountRandomRange) + 1;
    }

    
}
