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
            Counter.Instance.packingLine.EnqueueCustomer(customer);
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
        private PaperBag bag;
        public IsPacking(Customer customer)
        {
            this.customer = customer;
        }

        public void OnEnter()
        {
            bag = PaperBagPoolManager.Instance.GetPaperBag();
            bag.transform.SetParent(Counter.Instance.GetPaperBagPos());
            bag.transform.localRotation = Quaternion.identity;
            bag.transform.localScale = Vector3.one;
            bag.transform.localPosition = Vector3.zero;
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
            Counter.Instance.packingLine.RePosCustomers();
            Counter.Instance.Packing_Pay(customer);

        }
        public void OnPack()
        {
            if (!customer.isStakcing && customer.StackCount > 0)
            {
                Bread bread = customer.PopBread();
                customer.priceToPay += bread.price;
                SetBreadTransform(bread.transform);
                customer.StartCoroutine(customer.CorStackAnim(bread.transform, customer.GetStackStartPos, Counter.Instance.PaperBagPos, 20.0f, 0.01f));
            }
        }
        private bool OnPackComplete()
        {
            if (!bag.IsPacking)
            {
                if (customer.StackCount == 0)
                {
                    bag.PlayPackAnimation();
                }
            }
            else
            {
                if (bag.IsPackOver)
                {
                    if (!customer.isStakcing)
                    {
                        customer.StartCoroutine(customer.CorStackAnim(bag.transform, Counter.Instance.PaperBagPos, customer.GetStackDestPos, 20.0f, 0.01f));
                        bag.transform.Rotate(0, 90, 0);
                    }
                    else
                    {
                        customer.ForceSetIsStackAnim();
                        bag.transform.SetParent(customer.transform);
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
            bread.SetParent(bag.transform, false);
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
        }
        public bool EvaluateCompleteCondition()
        {
            return TableManager.Instance.GetTableAvailable() != null;
        }

        public void OnReached()
        {

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
            customer.transform.rotation = Quaternion.Euler(0, 180, 0);
            customer.anim.SetBool("isSitting",true);
            // 앉는 애니메이션으로 변경
        }
        public void OnComplete()
        {
            customer.anim.SetBool("isSitting", false);
            table.OnEndEatingTable(customer);
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
}