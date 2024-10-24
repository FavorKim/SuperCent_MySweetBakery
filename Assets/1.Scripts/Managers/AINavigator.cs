using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigator : Singleton<AINavigator>
{
    [SerializeField] private SaleShelves shelves;

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

}
