using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
public class Customer : BreadStacker
{
    #region Variable
    protected WaitingPosition posToGo;

    private NavMeshAgent agent;
    private CustomerNeedsManager needsManager;
    protected CustomerUIManager UIManager { get; private set; }

    public bool isPacking;

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

    // 나중에 테이블 매니저가 나오면 교체할 가용 테이블이 있는지에 대한 변수
    public bool isTableAvailable = false;
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
        needsManager.RunNeedsQueue();

        SetAnimatorOnMove();
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
        if (BreadCountToNeed > 0 && IsReached)
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
        if (needsManager == null)
            needsManager = new CustomerNeedsManager();
        needsManager.EnqueueNeeds(new NeedBread(this));
        needsManager.EnqueueNeeds(new NeedPay(this));
        needsManager.EnqueueNeeds(new NeedPacking(this));
        needsManager.EnqueueNeeds(new IsPacking(this));
        needsManager.EnqueueNeeds(new GoBack(this));

        transform.position = DestinationManager.Instance.GetEntrancePos();
    }
    public void OnEndCustomerAI()
    {

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
    private void SetAnimatorOnMove()
    {
        bool isStop = AgentIsMove();
        anim.SetBool("isMove", isStop);
    }
    public int GetPriceToPay()
    {
        return priceToPay;
    }
    #endregion



    #region CustomerNeeds
    public class NeedBread : CustomerNeeds
    {
        private Customer customer;
        public NeedBread(Customer customer)
        {
            this.customer = customer;
        }

        public void OnEnter()
        {
            OnEnter_SetDestination();
            OnEnter_SetBreadCount();
        }
        public bool EvaluateCompleteCondition()
        {
            return customer.BreadCountToNeed == 0;
        }
        public void OnReached()
        {
            OnReached_SetBreadUI();
            OnReached_SetDestination();
            customer.isStakcing = false;
        }
        public void OnComplete()
        {
            OnComplete_Bread();
        }


        private void OnEnter_SetBreadCount()
        {
            customer.BreadCountToNeed = Random.Range(0, customer.breadCountRandomRange) + 1;
            customer.stackMaxCount = customer.BreadCountToNeed;
        }

        private void OnEnter_SetDestination()
        {
            customer.isStakcing = true;
            customer.posToGo = DestinationManager.Instance.GetSaleShelvesWaitingPos();
            customer.AINavMoveToward(customer.posToGo.GetPosition());
        }


        private void OnReached_SetDestination()
        {
            Vector3 shelvPos = DestinationManager.Instance.GetSaleShelvesPos();
            customer.transform.rotation = Quaternion.LookRotation(shelvPos);
        }

        private void OnReached_SetBreadUI()
        {
            customer.UIManager.SetActiveUI(UIType.BREAD, true);
            customer.BreadCountToNeed = customer.stackMaxCount - customer.StackCount;
            customer.UIManager.SetBreadText($"{customer.BreadCountToNeed}");
        }



        private void OnComplete_Bread()
        {
            customer.UIManager.SetActiveUI(UIType.BREAD, false);
            customer.posToGo.SetAvailable(true);
            customer.posToGo = null;
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
            OnEnter_SetDestination();

        }
        public bool EvaluateCompleteCondition()
        {
            return Counter.Instance.isPayable && customer.IsReached;
        }
        public void OnReached()
        {
            OnReached_Pay();
        }

        public void OnComplete()
        {

        }

        public void OnReached_Pay()
        {
            customer.UIManager.SetActiveUI(UIType.PAY, true);
            customer.transform.rotation = Quaternion.LookRotation(Vector3.back);
            Counter.Instance.EnqueueCustomer(customer);
        }
        private void OnEnter_SetDestination()
        {
            Vector3 posToWait = Counter.Instance.GetPosToWait();
            customer.AINavMoveToward(posToWait);
            customer.transform.rotation = Quaternion.LookRotation(posToWait);
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
            return customer.isPacking && customer.IsReached;
        }
        public void OnReached()
        {

        }

        public void OnComplete()
        {
        }

        

    }
    public class IsPacking : CustomerNeeds
    {
        private Customer customer;
        public IsPacking(Customer customer)
        {
            this.customer = customer;
        }

        public void OnEnter()
        {

        }
        public bool EvaluateCompleteCondition()
        {
            OnPack();

            return customer.StackCount == 0;
        }

        public void OnReached()
        {

        }
        public void OnComplete()
        {
            Counter.Instance.RePosCustomers();
            Counter.Instance.Pay(customer);

        }
        public void OnPack()
        {
            if (!customer.isStakcing && customer.StackCount > 0)
            {
                Bread bread = customer.PopBread();
                customer.priceToPay += bread.price;
                customer.StartCoroutine(customer.CorStackAnim(bread.transform, customer.GetStackStartPos, Counter.Instance.PaperBagPos));
            }
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
            OnEnter_SetDestination();
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

        private void OnEnter_SetDestination()
        {
            Vector3 entrance = DestinationManager.Instance.GetEntrancePos();
            customer.AINavMoveToward(entrance);
        }
    }
    #endregion
}
