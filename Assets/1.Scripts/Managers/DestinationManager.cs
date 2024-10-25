using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestinationManager : Singleton<DestinationManager>
{
    [SerializeField] private SaleShelves shelves;
    [SerializeField] private Transform entrance;
    public WaitingPosition GetSaleShelvesWaitingPos()
    {
        WaitingPosition dest = shelves.GetWaitingPositionAvailable();

        if (dest == null)
        {
            Debug.LogError("빈자리 없음");
            return null;
        }
        else
            return dest;
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
