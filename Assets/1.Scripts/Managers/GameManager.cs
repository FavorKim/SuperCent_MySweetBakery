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
        var customer = CustomerPoolManager.Instance.GetCustomer();
        customer.OnStartCustomerAI();
        delayedTime = 0;
    }

    private void Update()
    {
        delayedTime += Time.deltaTime;
        if (delayedTime >= entranceTime)
            SetCustomer();
    }


}
