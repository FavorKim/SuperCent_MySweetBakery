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
    [SerializeField] private float stackDelay = 0.5f;
    [SerializeField] protected int stackMaxCount = 8;
    protected bool isStakcing = false;

    [SerializeField] private Transform stackPos;
    [SerializeField] private Transform stackStartPos;
    protected int stackCount => breadStack.Count;

    protected virtual void OnEnable()
    {
        OnPushBread += OnPushBread_PushBread;
        OnPushBread += OnPushBread_PlayStackAnimation;
        OnPushBread += OnPushBread_SetPlayerAnim;

        OnPopBread += OnPopBread_SetBreadTransform;
        OnPopBread += OnPopBread_SetPlayerAnim;
    }
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void OnDisable()
    {
        OnPushBread -= OnPushBread_SetPlayerAnim;
        OnPushBread -= OnPushBread_PlayStackAnimation;
        OnPushBread -= OnPushBread_PushBread;

        OnPopBread -= OnPopBread_SetPlayerAnim;
        OnPopBread -= OnPopBread_SetBreadTransform;
    }

    private void OnPopBread_SetBreadTransform(Bread bread)
    {
        bread.transform.SetParent(null);
        bread.OnPopped();
    }
    private void OnPopBread_SetPlayerAnim(Bread bread)
    {
        anim.SetBool("isStack", false);
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
    private void OnPushBread_SetPlayerAnim(Bread bread)
    {
        anim.SetBool("isStack", true);
    }


    protected virtual void OnTriggerStorage(BreadStorage storage)
    {

    }
    protected virtual void OnTriggerSaleShelves(SaleShelves shelves)
    {

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
        if (stackCount < stackMaxCount)
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

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out BreadStorage storage))
        {
            OnTriggerStorage(storage);
        }
        if (other.TryGetComponent(out SaleShelves shelves))
        {
            OnTriggerSaleShelves(shelves);
        }
    }

}
