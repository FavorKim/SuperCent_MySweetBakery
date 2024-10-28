using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class MoneyView : MonoBehaviour
{
    [SerializeField] TMP_Text goldAmount;
    private StringBuilder moneyAmountSB = new StringBuilder();

    private void Start()
    {
        MoneyModel.Instance.OnPlusGold += OnPlusGold_SetGoldText;
        MoneyModel.Instance.RefreshGold();
    }


    private void OnPlusGold_SetGoldText(int curGold)
    {
        moneyAmountSB.Clear();
        moneyAmountSB.Append($"{curGold}");
        goldAmount.text = moneyAmountSB.ToString();
    }

    private void OnApplicationQuit()
    {
        MoneyModel.Instance.OnPlusGold -= OnPlusGold_SetGoldText;
    }
}
