using UnityEngine;

public enum BreadType 
{
    CROASSANT,
}

public abstract class Bread : MonoBehaviour, IBakeable, IStackable
{
    public BreadType breadName;
    protected Rigidbody rb;
    [SerializeField] protected float bakeForce;
    public int price;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public virtual void OnBaked() 
    {
        rb.AddForce(transform.forward * bakeForce, ForceMode.Impulse);
    }
    public virtual void OnPushed()
    {
        rb.isKinematic = true;
    }
    public virtual void OnPopped()
    {
        rb.isKinematic = true;
    }
    public abstract void SetBreadType();
}
