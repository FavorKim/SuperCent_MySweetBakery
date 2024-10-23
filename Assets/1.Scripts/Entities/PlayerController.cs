using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControllStick stick;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;


    private void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        Vector3 moveDir = GetMoveDir();

        MoveTowards(moveDir);
    }



    private Vector3 GetMoveDir()
    {
        Vector2 dir = stick.GetInputVector();
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
        return moveDir;
    }

    private void MoveTowards(Vector3 direction)
    {

        if (direction.sqrMagnitude > 0.001f)  // 방향 벡터가 거의 0이 아닌 경우
        {
            // Y축만 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 회전값 보간
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        }

        // 플레이어 이동
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
