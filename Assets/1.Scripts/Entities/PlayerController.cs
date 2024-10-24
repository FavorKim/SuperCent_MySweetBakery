using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControllStick stick;
    [SerializeField] private Transform stackPos;
    [SerializeField] private Transform stackStartPos;

    private NavMeshAgent agent;
    private Animator anim;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;

    private Stack<Bread> breadStack = new Stack<Bread>();
    private float stackHeight = 0.3f;
    [SerializeField] private float stackDelay = 0.5f;
    [SerializeField] private int stackMaxCount = 8;
    private bool isStakcing = false;

    private int stackCount => breadStack.Count;

    private event Action<Bread> OnPushBread;
    private event Action<Bread> OnPopBread;
    private event Action<Vector3> OnMove;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        OnPushBread += OnPushBread_PushBread;
        OnPushBread += OnPushBread_PlayStackAnimation;
        OnPushBread += OnPushBread_SetPlayerAnim;

        OnPopBread += OnPopBread_SetBreadTransform;
        OnPopBread += OnPopBread_SetPlayerAnim;

        OnMove += OnMove_MoveTowards;
        OnMove += OnMove_SetPlayerAnim;
    }

    private void Update()
    {
        PlayerMove();
    }

    private void OnDisable()
    {
        OnPushBread -= OnPushBread_SetPlayerAnim;
        OnPushBread -= OnPushBread_PlayStackAnimation;
        OnPushBread -= OnPushBread_PushBread;

        OnPopBread -= OnPopBread_SetPlayerAnim;
        OnPopBread -= OnPopBread_SetBreadTransform;

        OnMove -= OnMove_SetPlayerAnim;
        OnMove -= OnMove_MoveTowards;
    }






    #region events
    private void OnMove_MoveTowards(Vector3 direction)
    {

        if (direction.sqrMagnitude > 0.001f)  // 방향 벡터가 거의 0이 아닌 경우
        {
            // Y축만 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 회전값 보간
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        }

        // 플레이어 이동
        agent.Move(direction * moveSpeed * Time.deltaTime);
    }
    private void OnMove_SetPlayerAnim(Vector3 dir)
    {
        bool isMove = dir == Vector3.zero ? false : true;
        anim.SetBool("isMove", isMove);
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
    public void OnPushBread_PushBread(Bread bread)
    {
        bread.OnPushed();
        breadStack.Push(bread);
    }
    private void OnPushBread_SetPlayerAnim(Bread bread)
    {
        anim.SetBool("isStack", true);
    }
    #endregion


    private void OnTriggerStorage(BreadStorage storage)
    {
        if (!isStakcing)
        {
            Bread bread = storage.OnPopBread();
            if (bread != null)
            {
                if (breadStack.Count < stackMaxCount)
                    OnPushBread.Invoke(bread);
            }
        }
    }
    private void OnTriggerSaleShelves(SaleShelves shelves)
    {
        if (shelves.IsStackable() && !isStakcing)
        {
            Bread bread = PopBread();
            if (bread != null)
            {
                shelves.OnStackBread(bread);
                StartCoroutine(CorStackAnim(bread.transform, GetStackStartPos, shelves.GetPosToStack));
            }
        }
    }
    #region Method

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

    private Vector3 GetMoveDir()
    {
        Vector2 dir = stick.GetInputVector();
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
        return moveDir;
    }
    private Vector3 GetStackDestPos()
    {
        float height = stackHeight * breadStack.Count;
        Vector3 destPos = new Vector3(stackPos.position.x, stackPos.position.y + height, stackPos.position.z);
        return destPos;
    }
    private Vector3 GetStackStartPos()
    {
        float yGap = breadStack.Count * stackHeight;

        Vector3 startPos = new Vector3(stackStartPos.position.x, stackStartPos.position.y + yGap, stackStartPos.position.z);
        return startPos;
    }

    private void PlayerMove()
    {
        Vector3 moveDir = GetMoveDir();

        OnMove.Invoke(moveDir);
    }


    #endregion

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


    private IEnumerator CorStackAnim(Transform bread, Func<Vector3> startPos, Func<Vector3> destPos)
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

}
