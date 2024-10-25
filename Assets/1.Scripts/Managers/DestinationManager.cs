using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestinationManager : Singleton<DestinationManager>
{
    [SerializeField] private SaleShelves shelves;
    [SerializeField] private Transform entrance;
    public Vector3 GetSaleShelvesWaitingPos()
    {
        Vector3 dest = shelves.GetWaitingPositionAvailable();
        if (dest != Vector3.zero)
            return dest;
        else
        {
            Debug.LogError("진열대 빈 자리 없음");
            return Vector3.zero;
        }
    }
    public Vector3 GetSaleShelvesPos()
    {
        return shelves.transform.position;
    }
    protected override void OnAwake() { }

    public Vector3 GetEntrancePos()
    {
        return entrance.position;
    }
}
