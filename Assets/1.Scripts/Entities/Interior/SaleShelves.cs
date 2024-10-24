using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPosition
{
    private Vector3 position;
    public bool isAvailable { get; private set; }

    public WaitingPosition(Vector3 pos)
    {
        position = pos;
        isAvailable = true;
    }

    public Vector3 GetPosition()
    {
        isAvailable = false;
        return position;
    }
}

public class SaleShelves : MonoBehaviour
{
    [SerializeField] private Transform firstPos;
    [SerializeField] List<Transform> waitingPositions;
    private List<WaitingPosition> waitingPositionsList = new List<WaitingPosition>();

    [SerializeField] private int maxStoreCount = 8;
    
    [SerializeField] private float xGap;
    [SerializeField] private float zGap;
    [SerializeField] private float yGap = 0.5f;

    [SerializeField] private float stackSpeed = 20.0f;

    private Stack<Bread> breadStacks = new Stack<Bread>();


    private void InitWaitingPositions()
    {
        foreach(Transform t in waitingPositions)
        {
            waitingPositionsList.Add(new WaitingPosition(t.position));
        }
    }

    public Vector3 GetWaitingPositionAvailable()
    {
        if( waitingPositionsList.Count == 0)
        {
            InitWaitingPositions();
        }

        foreach (WaitingPosition w in waitingPositionsList)
        {
            if (w.isAvailable) 
            {
                return w.GetPosition();
            }
        }

        return Vector3.zero;
    } 

    public void OnStackBread(Bread bread)
    {
        if (breadStacks.Count < maxStoreCount)
        {
            breadStacks.Push(bread);
            bread.transform.SetParent(firstPos);
        }
    }

    public Vector3 GetPosToStack()
    {
        int index = breadStacks.Count - 1;
        float xGap = (index % 2) * this.xGap;
        float zGap = (index / 2) * this.zGap;
        float yGap = (index / 2) * this.yGap;
        Vector3 pos = new Vector3(firstPos.position.x + xGap, firstPos.position.y - yGap, firstPos.position.z - zGap);
        return pos;
    }

    

    public Bread PopBread()
    {
        if (breadStacks.Count > 0)
            return breadStacks.Pop();
        else
        {
            return null;
        }
    }
    public bool IsStackable()
    {
        return breadStacks.Count < maxStoreCount;
    }
}
