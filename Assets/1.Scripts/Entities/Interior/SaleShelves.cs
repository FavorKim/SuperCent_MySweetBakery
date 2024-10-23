using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaleShelves : MonoBehaviour
{
    [SerializeField] private Transform firstPos;
    [SerializeField] private float xGap;
    [SerializeField] private float zGap;

    [SerializeField] private float stackSpeed = 20.0f;

    Stack<Bread> breadStacks = new Stack<Bread>();

    public void OnStackBread(Bread bread)
    {
        Vector3 pos = GetPosToStack();

        StartCoroutine(CorStackAnimation(bread.transform, pos, stackSpeed));

        breadStacks.Push(bread);
    }

    private Vector3 GetPosToStack()
    {
        int index = breadStacks.Count;
        float xGap = (index % 2) * this.xGap;
        float zGap = (index / 2) * this.zGap;
        Vector3 pos = new Vector3(firstPos.position.x + xGap, firstPos.position.y, firstPos.position.z - zGap);
        return pos;
    }

    private IEnumerator CorStackAnimation(Transform bread, Vector3 toward, float stackSpeed)
    {
        while ((bread.transform.position - toward).sqrMagnitude > 0.001f)
        {
            bread.transform.position = Vector3.Slerp(bread.transform.position, toward, Time.deltaTime * stackSpeed);
            bread.transform.localRotation = Quaternion.Slerp(bread.transform.rotation, firstPos.rotation, Time.deltaTime * stackSpeed);
            yield return null;
        }
        bread.transform.position = toward;
        bread.localRotation = firstPos.rotation;
    }
}
