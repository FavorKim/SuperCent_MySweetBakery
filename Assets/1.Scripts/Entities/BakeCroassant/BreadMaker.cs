using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadMaker : MonoBehaviour
{
    [SerializeField] private BreadType BreadType;
    [SerializeField] private Transform spawnPos;

    [SerializeField] private float bakeSpeed = 1.0f;

    [SerializeField] private int StorageSize = 8;
    [SerializeField] private int storedBreadCount;

    private void Start()
    {
        StartCoroutine(CorBakeBread());
    }

    private IEnumerator CorBakeBread()
    {
        while (true)
        {
            if (storedBreadCount < StorageSize)
            {
                Bread bread = GenerateBread();

                yield return new WaitForSeconds(0.5f);

                OnCompleteBake(bread);

                yield return new WaitForSeconds(bakeSpeed);
            }
            else
            {
                yield return null;
            }
        }
    }

    private Bread GenerateBread()
    {
        Bread bread = BreadPoolManager.Instance.GetBread(BreadType);
        bread.transform.position = spawnPos.position;
        bread.transform.rotation = spawnPos.rotation;
        return bread;
    }

    private void OnCompleteBake(Bread bread)
    {
        bread.OnBaked();
        storedBreadCount++;
    }
}
