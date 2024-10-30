using UnityEngine;
using UnityEngine.Events;

public class ArrowPos
{
    public UnityEvent IsTutorialCleared;
    public Vector3 position;

    public ArrowPos(UnityEvent isTutorialCleared, Vector3 position)
    {
        IsTutorialCleared = isTutorialCleared;
        this.position = position;
    }
}
