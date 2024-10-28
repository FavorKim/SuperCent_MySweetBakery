using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject Trash;

    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private ParticleSystem cleanVFX;

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

    public void OnEndEatingTable(Customer customer)
    {
        Trash.SetActive(true);
        InstanceMoney(customer);
    }
    public void OnCleanTable()
    {
        Trash.SetActive(false);
        IsAvailable = true;
        cleanVFX.Play();
    }

    private void InstanceMoney(Customer customer)
    {
        moneyManager.InstanceMoney(customer.GetPriceToPay() + 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if (Trash.activeSelf == true)
                OnCleanTable();
        }
    }
}
