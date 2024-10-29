using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float entranceTime = 15.0f;
    private float delayedTime = 15.0f;

    public ResourceManager<ParticleSystem> VFXManager = new ResourceManager<ParticleSystem>("VFX");
    public ResourceManager<AudioClip> SFXManager = new ResourceManager<AudioClip>("SFX");

    void SetCustomer()
    {
        if (DestinationManager.Instance.GetSaleShelvesWaitingPos() != null)
        {
            var customer = CustomerPoolManager.Instance.GetCustomer();
            customer.OnStartCustomerAI();
        }
        else
        {
            Debug.LogError("웨이팅이 너무 많아서 손님이 나오지 않음");
        }
        delayedTime = 0;
    }

    private void Update()
    {
        delayedTime += Time.deltaTime;
        if (delayedTime >= entranceTime)
            SetCustomer();
    }


}
