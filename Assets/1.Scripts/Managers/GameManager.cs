using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject PointCam_Unlock1;
    [SerializeField] private GameObject PointCam_Unlock2;
    private float pointDuration = 1.5f;


    [SerializeField] private float entranceTime = 15.0f;
    private float delayedTime = 15.0f;

    public ResourceManager<ParticleSystem> VFXManager = new ResourceManager<ParticleSystem>("VFX");
    public ResourceManager<AudioClip> SFXManager = new ResourceManager<AudioClip>("SFX");

    public bool IsGameStop { get; set; }

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

    public void PointCamUnlockActive(int index)
    {
        if (!TutorialInfo.Instance.HavePointed(index))
        {
            GameObject pointCam = index == 1 ? PointCam_Unlock1 : PointCam_Unlock2;
            StartCoroutine(CorPointUnlockCam(pointCam));
            TutorialInfo.Instance.Point(index);
        }
    }

    IEnumerator CorPointUnlockCam(GameObject obj)
    {
        obj.SetActive(true);
        IsGameStop = true;
        yield return new WaitForSeconds(pointDuration);
        obj.SetActive(false);
        IsGameStop = false;
    }
}
