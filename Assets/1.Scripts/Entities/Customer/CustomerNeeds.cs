using UnityEngine;
using static UnityEditor.PlayerSettings;

public interface CustomerNeeds
{
    void OnEnter();
    void OnReached();
    bool EvaluateCompleteCondition();
    void OnComplete();
}

public partial class Customer : BreadStacker
{
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
            customer.UIManager.OffAllUI();

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
            Counter.Instance.packingLine.EnqueueCustomer(customer);
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
            customer.UIManager.OffAllUI();
        }

        public void OnReached_Pay()
        {
            customer.UIManager.SetActiveUI(UIType.PAY, true);
            customer.transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
        private void OnEnter_SetDestination()
        {
            Vector3 posToWait = Counter.Instance.packingLine.GetPosToWait();
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
            customer.bag = PaperBagPoolManager.Instance.GetPaperBag();
            customer.bag.transform.SetParent(Counter.Instance.GetPaperBagPos());
            customer.bag.transform.localRotation = Quaternion.identity;
            customer.bag.transform.localScale = Vector3.one;
            customer.bag.transform.localPosition = Vector3.zero;
        }
        public bool EvaluateCompleteCondition()
        {
            OnPack();

            return OnPackComplete();
        }

        public void OnReached()
        {

        }
        public void OnComplete()
        {
            Counter.Instance.Packing_Pay(customer);
            Counter.Instance.packingLine.RePosCustomers();
            customer.UIManager.OffAllUI();
        }
        public void OnPack()
        {
            if (!customer.isStakcing && customer.StackCount > 0)
            {
                Bread bread = customer.PopBread();
                customer.priceToPay += bread.price;
                SetBreadTransform(bread.transform);
                customer.StartCoroutine(customer.CorStackAnim(bread.transform, customer.GetStackStartPos, Counter.Instance.PaperBagPos, 10.0f, 0.1f));
            }
        }
        private bool OnPackComplete()
        {
            if (!customer.bag.IsPacking)
            {
                if (customer.StackCount == 0)
                {
                    customer.bag.PlayPackAnimation();
                }
            }
            else
            {
                if (customer.bag.IsPackOver)
                {
                    if (!customer.isStakcing)
                    {
                        customer.StartCoroutine(customer.CorStackAnim(customer.bag.transform, Counter.Instance.PaperBagPos, customer.GetStackDestPos, 10.0f, 0.01f));
                        customer.bag.transform.Rotate(0, 90, 0);
                    }
                    else
                    {
                        customer.ForceSetIsStackAnim();
                        customer.bag.transform.SetParent(customer.transform);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SetBreadTransform(Transform bread)
        {
            bread.SetParent(null);
            bread.transform.localRotation = Quaternion.Euler(0, 90, 0);
            bread.SetParent(customer.bag.transform, false);
            bread.localScale = Vector3.one;
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
            Vector3 dest = Counter.Instance.tableLine.GetPosToWait();
            customer.AINavMoveToward(dest);
            Counter.Instance.tableLine.EnqueueCustomer(customer);
        }
        public bool EvaluateCompleteCondition()
        {
            return TableManager.Instance.GetTableAvailable() != null;
        }

        public void OnReached()
        {
            customer.UIManager.SetActiveUI(UIType.TABLE, true);
            customer.transform.rotation = Quaternion.LookRotation(Vector3.back);

        }
        public void OnComplete()
        {
            Counter.Instance.tableLine.RePosCustomers();
        }
    }
    public class EatingAtTable : CustomerNeeds
    {
        private Customer customer;
        private Table table;
        public EatingAtTable(Customer customer)
        {
            this.customer = customer;
        }

        public void OnEnter()
        {
            table = TableManager.Instance.GetTableAvailable();
            table.IsAvailable = false;
            customer.AINavMoveToward(table.ChairPos.position);
            
        }
        public bool EvaluateCompleteCondition()
        {
            customer.currentEatingTime += Time.deltaTime;
            bool isEatEnd = customer.currentEatingTime >= customer.entireEatingTime;

            return isEatEnd;
        }

        public void OnReached()
        {
            customer.transform.rotation = Quaternion.LookRotation(Vector3.back);
            customer.anim.SetBool("isSitting",true);
            customer.UIManager.OffAllUI();
        }
        public void OnComplete()
        {
            customer.anim.SetBool("isSitting", false);
            table.OnEndEatingTable(customer);
        }

        private void OnComplete_ResetBread()
        {
            for(int i = 0; i < customer.StackCount; i++)
            {
                Bread bread = customer.PopBread();
                BreadPoolManager.Instance.ReturnBread(bread);
            }
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
            customer.UIManager.SetPlesure();
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
}