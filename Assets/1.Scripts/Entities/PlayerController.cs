using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class PlayerController : BreadStacker
{
    [SerializeField] private ControllStick stick;
    

    private NavMeshAgent agent;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;

    

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
    }

    private void Update()
    {
        PlayerMove();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
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
        agent.Move(direction * Time.deltaTime* moveSpeed);
    }
    private void OnMove_SetPlayerAnim(Vector3 dir)
    {
        bool isMove = dir == Vector3.zero ? false : true;
        anim.SetBool("isMove", isMove);
    }

    
    #endregion


    protected override void OnTriggerStorage(BreadStorage storage)
    {
        if (!isStakcing)
        {
            Bread bread = storage.OnPopBread();
            if (bread != null)
            {
                if (stackCount < stackMaxCount)
                    InvokeOnPushBread(bread);
            }
        }
    }
    protected override void OnTriggerSaleShelves(SaleShelves shelves)
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
