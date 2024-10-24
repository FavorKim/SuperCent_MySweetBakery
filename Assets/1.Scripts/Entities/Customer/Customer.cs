using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Customer : BreadStacker
{
    private NavMeshAgent agent;
    private CustomerNeedsManager needsManager;
    public CustomerUIManager UiManager { get; private set; }

    [SerializeField] private float rotSpeed = 20.0f;
    
    public bool IsReadyToPay { get; private set; }


    [SerializeField] private int breadCountRandomRange = 3;
    public int BreadCountToNeed { get; private set; }
    
    // 나중에 테이블 매니저가 나오면 교체할 가용 테이블이 있는지에 대한 변수
    public bool isTableAvailable = false;


    private bool isReached = false;
    public bool IsReached
    {
        get 
        {
            return isReached; 
        }
        set 
        {
            if (isReached != value)
            {
                isReached = value;
                if(isReached == true)
                {
                    needsManager.OnReached();
                }
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        UiManager = GetComponentInChildren<CustomerUIManager>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (needsManager == null)
            needsManager = new CustomerNeedsManager();
        needsManager.EnqueueNeeds(new NeedBread(this));
        needsManager.EnqueueNeeds(new NeedPay(this));

        OnPushBread += OnPushBread_SetCount;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        needsManager.ResetCustomerNeedsManager();

        OnPushBread -= OnPushBread_SetCount;

    }

    private void Update()
    {
        needsManager.RunNeedsQueue();
        SetAnimatorOnMove();
    }


    protected override void OnTriggerSaleShelves(SaleShelves shelves)
    {
        if (!isStakcing && stackCount<stackMaxCount)
        {
            Bread bread = shelves.PopBread();
            if (bread != null)
            {
                InvokeOnPushBread(bread);
            }
        }
    }


    public void AINavMoveToward(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    public void SetBreadCountRandomly()
    {
        BreadCountToNeed = Random.Range(0, breadCountRandomRange) + 1;
        stackMaxCount = BreadCountToNeed;
    }

    private void SetAnimatorOnMove()
    {
        bool isStop = AgentIsMove();
        anim.SetBool("isMove", isStop);
    }

    private bool AgentIsMove()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                IsReached = true;
                return false;
            }
        }
        IsReached = false;
        return true;
    }

    public void RotateToward(Vector3 direction)
    {
        StartCoroutine(CorRotateToward(direction));
    }


    public void OnReached_Bread()
    {
        UiManager.SetActiveUI(UIType.BREAD, true);
        
        BreadCountToNeed = stackMaxCount - stackCount;
        UiManager.SetBreadText($"{BreadCountToNeed}");
        isStakcing = false;
    }

    private void OnPushBread_SetCount(Bread bread)
    {
        BreadCountToNeed = stackMaxCount - stackCount;

        UiManager.SetBreadText($"{BreadCountToNeed}");
    }

    public void OnEnter_Bread()
    {
        isStakcing = true;
    }

    public void OnComplete_Bread()
    {
        UiManager.SetActiveUI(UIType.BREAD, false);
    }

    public void OnEnter_Pay()
    {
        UiManager.SetActiveUI(UIType.PAY, true);
    }

    private IEnumerator CorRotateToward(Vector3 direction)
    {
        while (direction.sqrMagnitude > 0.001f)  // 방향 벡터가 거의 0이 아닌 경우
        {
            // Y축만 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 회전값 보간
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
            yield return null;
        }
    }
}
