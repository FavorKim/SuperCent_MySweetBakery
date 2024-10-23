using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ControllStick stick;
    [SerializeField] private Transform stackPos;
    [SerializeField] private Transform stackStartPos;
    private NavMeshAgent agent;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;

    private Stack<Bread> breadStack = new Stack<Bread>();
    private float stackHeight = 0.3f;
    [SerializeField] private float stackSpeed = 0.5f;
    [SerializeField] private int stackMaxCount = 8;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

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
        agent.Move(direction * moveSpeed * Time.deltaTime);
    }

    public void PushBread(Bread bread)
    {
        if (breadStack.Count < stackMaxCount)
        {
            bread.OnPushed();
            PlayStackAnimation(bread);
            breadStack.Push(bread);
        }
    }
    public Bread PopBread()
    {
        if (breadStack.Count > 0)
        {

            Bread bread = breadStack.Pop();
            bread.transform.SetParent(null);
            bread.transform.position = stackStartPos.position;
            bread.OnPopped();
            return bread;
        }
        else
        {
            return null;
        }
    }

    private void PlayStackAnimation(Bread bread)
    {
        StartCoroutine(CorStackPopAnimation(bread.transform));
        
        bread.transform.SetParent(stackPos.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent(out BreadStorage storage))
        {
            Bread bread = storage.OnPopBread();
            if (bread != null)
            {
                PushBread(bread);
            }
        }
        if(other.TryGetComponent(out SaleShelves shelves))
        {
            Bread bread = PopBread();
            if (bread != null)
            {
                shelves.OnStackBread(bread);
            }
        }
    }

    private IEnumerator CorStackPopAnimation(Transform bread)
    {

        bread.transform.position = stackStartPos.position;

        float height = breadStack.Count * stackHeight;
        Vector3 pos = new Vector3(stackPos.position.x, stackPos.position.y + height, stackPos.position.z);
        
        while ((bread.transform.position - pos).sqrMagnitude > 0.001f)
        {
            pos = new Vector3(stackPos.position.x, stackPos.position.y + height, stackPos.position.z);
            bread.transform.position = Vector3.Slerp(bread.transform.position, pos, Time.deltaTime * stackSpeed);
            bread.transform.localRotation = Quaternion.Slerp(bread.transform.rotation, Quaternion.identity, Time.deltaTime * stackSpeed);

            yield return null;
        }
        bread.transform.position = pos;
        bread.localRotation = Quaternion.identity;
    }
    
    
}
