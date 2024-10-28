using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;


public enum UIType
{
    BREAD,
    PAY,
    TABLE,
    PLESURED
}

public class CustomerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject Ballon;
    [SerializeField] private GameObject BreadUI;
    [SerializeField] private TMP_Text Txt_Bread;
    [SerializeField] private GameObject PayUI;
    [SerializeField] private GameObject TableUI;
    [SerializeField] private ParticleSystem PleasureVFX;

    private Dictionary<UIType, GameObject> uITypeDict = new Dictionary<UIType, GameObject>();

    private StringBuilder BreadTextSB = new StringBuilder();
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
        Ballon.SetActive(true);
        BreadTextSB.Clear();
        BreadTextSB.Append(text);
        Txt_Bread.text = BreadTextSB.ToString();
    }

    public void SetActiveUI(UIType type, bool isActive)
    {
        Ballon.SetActive(isActive);
        UITypeDict[type].SetActive(isActive);
    }

    private void InitUITypeDict()
    {
        uITypeDict.Add(UIType.BREAD, BreadUI);
        uITypeDict.Add(UIType.PAY, PayUI);
        uITypeDict.Add(UIType.TABLE, TableUI);
    }

    public void SetPlesure()
    {
        Ballon.SetActive(false);
        foreach(GameObject ui in UITypeDict.Values)
        {
            ui.SetActive(false);
        }
        PleasureVFX.Play();
    }
    public void OffAllUI()
    {
        foreach (GameObject ui in UITypeDict.Values) 
        {
            ui.SetActive(false); 
        }
        Ballon.SetActive(false);
    }
    
}
