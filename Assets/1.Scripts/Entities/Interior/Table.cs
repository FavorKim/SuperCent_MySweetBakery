using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject Trash;

    private bool isAvailable = false;
    public bool IsAvailable
    {
        get { return isAvailable; }
        set
        {
            if (isAvailable != value)
                isAvailable = value;
        }
    }
    [SerializeField] private Transform chairPos;
    public Transform ChairPos { get { return chairPos; } }

    private void OnEnable()
    {
        IsAvailable = true;
        TableManager.Instance.AddTable(this);
    }

    public void OnEndEatingTable()
    {
        Trash.SetActive(true);
    }
    public void OnCleanTable()
    {
        Trash.SetActive(false);
        IsAvailable = true;
    }

}
