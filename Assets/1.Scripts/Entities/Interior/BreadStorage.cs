using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreadStorage : MonoBehaviour
{
    private Stack<Bread> breadStack = new Stack<Bread>();
    [SerializeField] private int StorageSize = 8;
    private UnityEvent OnTutorialCleared = new UnityEvent();

    private void Awake()
    {
        TutorialArrowController.Instance.AddCondition(OnTutorialCleared, 0);
    }

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
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if (TutorialArrowController.Instance.CurrentTutorialLevel == 1)
                OnTutorialCleared.Invoke();
        }
    }
}
