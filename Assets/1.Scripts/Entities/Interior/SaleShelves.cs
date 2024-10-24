using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaleShelves : MonoBehaviour
{
    [SerializeField] private Transform firstPos;

    [SerializeField] private int maxStoreCount = 8;
    
    [SerializeField] private float xGap;
    [SerializeField] private float zGap;
    [SerializeField] private float yGap = 0.5f;

    [SerializeField] private float stackSpeed = 20.0f;

    private Stack<Bread> breadStacks = new Stack<Bread>();


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
