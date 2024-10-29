using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
public partial class Customer : BreadStacker
{
    #region Variable
    private WaitingPosition posToGo;

    private NavMeshAgent agent;
    private CustomerNeedsManager needsManager;
    private CustomerUIManager UIManager;
    
    private PaperBag bag;

    public bool isReady;

    [SerializeField] private float rotSpeed = 20.0f;

    private bool isPayOver = false;

    [SerializeField] private int breadCountRandomRange = 3;

    public int BreadCountToNeed { get; protected set; }



    private int priceToPay = 0;


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
                if (isReached == true)
                {
                    needsManager.OnReached();
                }
            }
        }
    }

    [SerializeField] private float entireEatingTime;
    private float currentEatingTime;

    #endregion

    #region UnityLifeCycle
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        UIManager = GetComponentInChildren<CustomerUIManager>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        OnPushBread += OnPushBread_SetCount;
    }
    private void Update()
    {
        SetAnimatorOnMove();

    }
    private void FixedUpdate()
    {
        IsReached = !AgentIsMove();
        needsManager.RunNeedsQueue();

    }
    protected override void OnDisable()
    {
        base.OnDisable();

        OnPushBread -= OnPushBread_SetCount;

    }
    #endregion

    #region events
    protected override void OnTriggerStay_SaleShelves(SaleShelves shelves)
    {
        if (BreadCountToNeed > 0 && IsReached && shelves.IsCustomerUsable)
        {
            Bread bread = shelves.PopBread();
            if (bread != null)
            {
                InvokeOnPushBread(bread);
            }
        }
    }
    public void OnStartCustomerAI()
    {
        SetCustomerAI();

        transform.position = DestinationManager.Instance.GetEntrancePos();
    }
    public void OnEndCustomerAI()
    {
        if (bag != null)
            PaperBagPoolManager.Instance.ReturnPaperBag(bag);

        UIManager.OffAllUI();
        needsManager.ResetCustomerNeedsManager();
        priceToPay = 0;
        agent.isStopped = true;
        CustomerPoolManager.Instance.ReturnCustomer(this);
    }
    private void OnPushBread_SetCount(Bread bread)
    {
        BreadCountToNeed = stackMaxCount - StackCount;

        UIManager.SetBreadText($"{BreadCountToNeed}");
    }
    #endregion

    #region method
    public void AINavMoveToward(Vector3 pos)
    {
        agent.SetDestination(pos);
    }
    private bool AgentIsMove()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void SetAnimatorOnMove()
    {
        bool isStop = AgentIsMove();
        anim.SetBool("isMove", isStop);
    }
    public int GetPriceToPay()
    {
        return priceToPay;
    }

    private void EnqueueCustomerAIDefault()
    {
        needsManager.EnqueueNeeds(new NeedBread(this));
        needsManager.EnqueueNeeds(new NeedPay(this));
        needsManager.EnqueueNeeds(new NeedPacking(this));
        needsManager.EnqueueNeeds(new IsPacking(this));
        needsManager.EnqueueNeeds(new GoBack(this));
    }
    private void SetCustomerAITable()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
            EnqueueCustomerAIDefault();
        else
            EnqueueCustomerAITable();
    }
    private void EnqueueCustomerAITable()
    {
        needsManager.EnqueueNeeds(new NeedBread(this));
        needsManager.EnqueueNeeds(new NeedTable(this));
        needsManager.EnqueueNeeds(new EatingAtTable(this));
        needsManager.EnqueueNeeds(new GoBack(this));
    }

    private void SetCustomerAI()
    {
        if (needsManager == null)
            needsManager = new CustomerNeedsManager();

        if (MoneyModel.Instance.GoldCount > 30 ||
            TableManager.Instance.GetTableAvailable() != null)
            SetCustomerAITable();
        else
            EnqueueCustomerAIDefault();
    }
    #endregion




}
