using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private Vector3 Gap = new Vector3(-0.8f, 0.22f, 0.5f);

    [SerializeField] private Transform moneyFirstPos;

    private Stack<GameObject> moneyStack = new Stack<GameObject>();
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
}
