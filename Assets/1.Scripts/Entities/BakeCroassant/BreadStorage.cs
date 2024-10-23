using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadStorage : MonoBehaviour
{
    private Stack<Bread> breadStack = new Stack<Bread>();
    [SerializeField] private int StorageSize = 8;


    public void OnStoreBread(Bread bread)
    {
        breadStack.Push(bread);
    }

    public Bread OnPopBread()
    {
        if (breadStack.Count > 0)
            return breadStack.Pop();
        else
            return null;
    }

    public bool CanPush()
    {
        return breadStack.Count < StorageSize;
    }
}
