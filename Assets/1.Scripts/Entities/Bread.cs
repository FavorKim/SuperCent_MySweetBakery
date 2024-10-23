using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BreadType 
{
    CROASSANT,
}

public abstract class Bread : MonoBehaviour, IBakeable
{
    public BreadType breadName;
    protected Rigidbody rb;
    [SerializeField] protected float bakeForce;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public virtual void OnBaked() 
    {
        rb.AddForce(transform.forward * bakeForce, ForceMode.Impulse);
    }
    public abstract void SetBreadType();
}
