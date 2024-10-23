using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControllStick stick;
    [SerializeField] private float moveSpeed;


    private void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        Vector2 dir = stick.GetInputVector();
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);

        transform.Translate(moveDir * Time.deltaTime * moveSpeed);
    }
}
