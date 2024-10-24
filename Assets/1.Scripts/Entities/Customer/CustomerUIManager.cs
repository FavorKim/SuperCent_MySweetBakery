using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum UIType
{
    BREAD,
    PAY,
    TABLE,
}

public class CustomerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject BreadUI;
    [SerializeField] private TMP_Text Txt_Bread;
    [SerializeField] private GameObject PayUI;
    [SerializeField] private GameObject TableUI;

    private Dictionary<UIType, GameObject> uITypeDict = new Dictionary<UIType, GameObject>();

    public Dictionary<UIType,GameObject> UITypeDict 
    {
        get
        {
            if (uITypeDict.Count == 0)
                InitUITypeDict();
            return uITypeDict;
        }
    }
    

    public void SetBreadText(string text)
    {
        Txt_Bread.text = text;
    }

    public void SetActiveUI(UIType type, bool isActive)
    {
        UITypeDict[type].SetActive(isActive);
    }

    private void InitUITypeDict()
    {
        uITypeDict.Add(UIType.BREAD, BreadUI);
        uITypeDict.Add(UIType.PAY, PayUI);
        uITypeDict.Add(UIType.TABLE, TableUI);
    }
}
