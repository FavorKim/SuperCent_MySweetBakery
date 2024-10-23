using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour
{
    [SerializeField] private Croassant CroassantPrefab;
    [SerializeField] private int poolCount;

    private ObjectPool<Croassant> croassantPool;

    private void Awake()
    {
        croassantPool = new ObjectPool<Croassant>(CroassantPrefab, poolCount, transform);
    }
}
