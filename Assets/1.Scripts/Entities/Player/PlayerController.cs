using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BreadStacker
{
    [SerializeField] private ControllStick stick;


    private NavMeshAgent agent;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;

    [SerializeField] private GameObject MaxUI;



    private event Action<Vector3> OnMove;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnMove += OnMove_MoveTowards;
        OnMove += OnMove_SetPlayerAnim;

        OnPushBread += OnPushPopBread;
        OnPopBread += OnPushPopBread;

    }

    private void Update()
    {
        PlayerMove();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnPopBread -= OnPushPopBread;
        OnPushBread -= OnPushPopBread;

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
        agent.Move(direction * Time.deltaTime * moveSpeed);
    }
    private void OnMove_SetPlayerAnim(Vector3 dir)
    {
        bool isMove = dir == Vector3.zero ? false : true;
        anim.SetBool("isMove", isMove);
    }

    private void OnPushPopBread(Bread bread)
    {
        if (StackCount >= stackMaxCount)
            MaxUI.SetActive(true);
        else
            MaxUI.SetActive(false);
    }
    #endregion


    protected override void OnTriggerStay_Storage(BreadStorage storage)
    {
        Bread bread = storage.OnPopBread();
        if (bread != null)
        {
            InvokeOnPushBread(bread);
        }
    }
    protected override void OnTriggerStay_SaleShelves(SaleShelves shelves)
    {
        if (shelves.IsStackable())
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



    private Vector3 GetMoveDir()
    {
        Vector2 dir = stick.GetInputVector();
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
        return moveDir;
    }

    private void PlayerMove()
    {
        Vector3 moveDir = GetMoveDir();

        OnMove.Invoke(moveDir);
    }


    #endregion




}
