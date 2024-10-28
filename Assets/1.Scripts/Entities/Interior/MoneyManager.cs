using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private Vector3 Gap = new Vector3(-0.8f, 0.22f, 0.5f);

    [SerializeField] private Transform moneyFirstPos;

    private Stack<GameObject> moneyStack = new Stack<GameObject>();

    private float lerpSpeed = 50f;
    private float lerpDelay = 0.02f;

    private bool isEarning = false;
    private bool isLerping = false;

    public void InstanceMoney(int price)
    {
        for (int i = 0; i < price; i++)
        {
            var money = MoneyPoolManager.Instance.GetMoney();
            money.transform.SetParent(moneyFirstPos.parent);

            int xMulti = moneyStack.Count % 3;
            int yMulti = moneyStack.Count / 9;
            int zMulti = moneyStack.Count % 9 / 3;

            moneyStack.Push(money);
            Vector3 pos = new Vector3(xMulti * Gap.x, yMulti * Gap.y, zMulti * Gap.z);
            pos += moneyFirstPos.localPosition;

            money.transform.localPosition = pos;
        }
    }

    private void Update()
    {
        if (isEarning && moneyStack.Count > 0 && !isLerping)
        {
            StartCoroutine(CorEarnMoney());
        }
    }


    private IEnumerator CorEarnMoney()
    {
        isLerping = true;
        var money = moneyStack.Pop();
        Vector3 destPos = transform.position + Vector3.up * 5;

        while ((money.transform.position - destPos).sqrMagnitude > 0.1f)
        {
            money.transform.position = Vector3.Slerp(money.transform.position, destPos, Time.deltaTime * lerpSpeed);
            yield return null;
        }
        money.transform.position = destPos;
        MoneyPoolManager.Instance.ReturnMoney(money);
        MoneyModel.Instance.PlusGold(1);
        yield return new WaitForSeconds(lerpDelay);
        isLerping = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
            isEarning = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
            isEarning = false;
    }
}
