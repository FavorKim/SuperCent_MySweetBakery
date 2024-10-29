using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadStacker : MonoBehaviour
{
    protected Animator anim;

    
    protected event Action<Bread> OnPushBread;
    protected event Action<Bread> OnPopBread;

    private Stack<Bread> breadStack = new Stack<Bread>();
    
    private float stackHeight = 0.3f;
    private float stackDelay = 0.1f;
    private float stackLerpSpeed = 20.0f;
    
    protected int stackMaxCount = 8;
    protected bool isStakcing = false;

    [SerializeField] private Transform stackPos;
    [SerializeField] private Transform stackStartPos;

    private AudioClip SFX_GetObject;
    private AudioClip SFX_PutObject;

    protected AudioSource audioSource;

    public int StackCount
    {
        get
        {
            
            return breadStack.Count;
        }
    }


    protected virtual void OnEnable()
    {
        OnPushBread += OnPushBread_PushBread;
        OnPushBread += OnPushBread_PlayStackAnimation;
        OnPushBread += OnChangedBreadStack;
        OnPushBread += OnPushBread_PlaySFX;

        OnPopBread += OnPopBread_SetBreadTransform;
        OnPopBread += OnChangedBreadStack;
        OnPopBread += OnPopBread_PlaySFX;

    }
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        SFX_GetObject = GameManager.Instance.SFXManager.GetResource("Get_Object");
        SFX_PutObject = GameManager.Instance.SFXManager.GetResource("Put_Object");
    }

    protected virtual void OnDisable()
    {
        OnPushBread -= OnPushBread_PlaySFX;
        OnPushBread -= OnChangedBreadStack;
        OnPushBread -= OnPushBread_PlayStackAnimation;
        OnPushBread -= OnPushBread_PushBread;

        OnPopBread -= OnPopBread_PlaySFX;
        OnPopBread -= OnChangedBreadStack;
        OnPopBread -= OnPopBread_SetBreadTransform;
    }


    private void OnPopBread_SetBreadTransform(Bread bread)
    {
        bread.transform.SetParent(null);
        bread.OnPopped();
    }
    

    private void OnPushBread_PlayStackAnimation(Bread bread)
    {
        bread.transform.SetParent(stackPos.transform);

        StartCoroutine(CorStackAnim(bread.transform, GetStackStartPos, GetStackDestPos));
    }
    private void OnPushBread_PushBread(Bread bread)
    {
        bread.OnPushed();
        breadStack.Push(bread);
    }
    private void OnPushBread_PlaySFX(Bread bread)
    {
        audioSource.clip = SFX_GetObject;
        audioSource.Play();
    }

    private void OnPopBread_PlaySFX(Bread bread)
    {
        audioSource.clip = SFX_PutObject;
        audioSource.Play();
    }


    protected virtual void OnTriggerStay_Storage(BreadStorage storage) { }
    protected virtual void OnTriggerStay_SaleShelves(SaleShelves shelves) { }
    
    private void OnChangedBreadStack(Bread bread)
    {
        if (breadStack.Count == 0)
            anim.SetBool("isStack", false);
        else
            anim.SetBool("isStack", true);
    }
    public void ForceSetIsStackAnim()
    {
        anim.SetBool("isStack", true);
    }


    public Bread PopBread()
    {
        if (breadStack.Count > 0)
        {

            Bread bread = breadStack.Pop();
            OnPopBread.Invoke(bread);
            return bread;
        }
        else
        {
            return null;
        }
    }

    protected Vector3 GetStackDestPos()
    {
        float height = stackHeight * breadStack.Count;
        Vector3 destPos = new Vector3(stackPos.position.x, stackPos.position.y + height, stackPos.position.z);
        return destPos;
    }
    protected Vector3 GetStackStartPos()
    {
        float yGap = breadStack.Count * stackHeight;

        Vector3 startPos = new Vector3(stackStartPos.position.x, stackStartPos.position.y + yGap, stackStartPos.position.z);
        return startPos;
    }

    protected void InvokeOnPushBread(Bread bread)
    {
        if (StackCount < stackMaxCount)
            OnPushBread.Invoke(bread);
    }
    protected void InvokeOnPopBread(Bread bread)
    {
        OnPopBread.Invoke(bread);
    }


    protected IEnumerator CorStackAnim(Transform bread, Func<Vector3> startPos, Func<Vector3> destPos)
    {
        float curTime = 0;

        bread.transform.position = startPos.Invoke();
        isStakcing = true;
        yield return null;
        while (curTime < stackDelay)
        {
            curTime += Time.deltaTime;
            yield return null;
        }


        bread.transform.position = destPos.Invoke();
        bread.transform.localRotation = Quaternion.identity;

        isStakcing = false;
    }
    protected IEnumerator CorStackAnim(Transform bread, Func<Vector3> startPos, Func<Vector3> destPos, float lerpSpeed, float stackDelay = 0.1f)
    {
        float curTime = 0;

        isStakcing = true;
        yield return null;
        
        bread.transform.position = startPos.Invoke();

        while ((bread.transform.position - destPos.Invoke()).sqrMagnitude > 0.1f)
        {
            bread.transform.position = Vector3.Slerp(bread.transform.position, destPos.Invoke(), Time.deltaTime * lerpSpeed);
            yield return null;
        }

        while (curTime < stackDelay)
        {
            curTime += Time.deltaTime;
            yield return null;
        }

        bread.transform.position = destPos.Invoke();

        isStakcing = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out BreadStorage storage) && !isStakcing)
        {
            OnTriggerStay_Storage(storage);
        }
        if (other.TryGetComponent(out SaleShelves shelves) && !isStakcing)
        {
            OnTriggerStay_SaleShelves(shelves);
        }
    }
   


}
