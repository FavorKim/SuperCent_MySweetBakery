using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialArrowController : Singleton<TutorialArrowController>
{
    [SerializeField] private GameObject Arrow;
    [SerializeField] Transform playerArrow;

    private Queue<ArrowPos> posQueue = new Queue<ArrowPos>();
    private List<UnityEvent> conditionList = new List<UnityEvent>() { null, null, null, null, null };

    private ArrowPos currentPos;

    public int CurrentTutorialLevel => conditionList.Count - posQueue.Count;


    protected override void OnStart()
    {
        if (!TutorialInfo.Instance.IsTutorialEnd)
        {
            InitPosQueue();
            ProgressTutorial();
        }
    }

    private void Update()
    {
        if (!TutorialInfo.Instance.IsTutorialEnd && CurrentTutorialLevel <= 5)
        {
            Vector3 playerArrowLookAt = new Vector3(currentPos.position.x, 1, currentPos.position.z);
            playerArrow.LookAt(playerArrowLookAt);
        }
    }

    private void InitPosQueue()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (conditionList[i] != null)
            {
                Transform child = transform.GetChild(i);
                UnityEvent condition = conditionList[i];
                condition.AddListener(ProgressTutorial);
                ArrowPos pos = new ArrowPos(condition, child.position);
                posQueue.Enqueue(pos);
            }
            else
            {
                Debug.LogError($"{i}번째 ConditionList가 비어있습니다.");
            }
        }


    }

    private void ProgressTutorial()
    {
        if (posQueue.Count == 0)
        {
            Arrow.SetActive(false);
            playerArrow.gameObject.SetActive(false);
            TutorialInfo.Instance.EndTutorial();
            return;
        }

        currentPos = posQueue.Dequeue();
        Arrow.transform.position = currentPos.position;
    }

    public void AddCondition(UnityEvent condition, int index)
    {
        conditionList[index] = condition;
    }

    private void OnApplicationQuit()
    {
        conditionList.Clear();
    }
}
