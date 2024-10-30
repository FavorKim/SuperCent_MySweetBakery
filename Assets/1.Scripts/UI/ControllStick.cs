using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllStick : MonoBehaviour,  IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private RectTransform rect;
    [SerializeField] private RectTransform stick;   // 컨트롤 스틱 중앙 움직이는 Stick 부분
    [SerializeField] private RectTransform point;   // 플레이어가 실제로 터치하고있는 부분을 알리는 UI
    private GameObject controller;                  // 터치시에만 UI를 노출시키기 위해 활성/비활성화할 오브젝트의 부모

    [SerializeField,Range(10,50)] private float leverRange = 15.0f;     // Stick이 움직이는 범위

    private Vector2 inputVector;    // 컨트롤 스틱으로 설정한 벡터


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        controller = transform.GetChild(0).gameObject;
    }

    public void SetStickVector(PointerEventData pointer)
    {
        // 터치 위치에서 컨트롤 스틱의 벡터를 뺀 값 산출
        var inputDir = pointer.position - rect.anchoredPosition;

        // 설정한 LeverRange보다 먼 거리에 있으면 Clamp
        var clampedDir = inputDir.magnitude < leverRange ? inputDir : inputDir.normalized * leverRange;

        // Clamp된 값을 Stick위치와 입력 벡터값에 적용
        stick.anchoredPosition = clampedDir;
        inputVector = clampedDir / leverRange;

        // 플레이어가 터치한 위치에 UI 노출
        point.position = pointer.position;

    }

    public Vector2 GetInputVector() 
    {
        if(GameManager.Instance.IsGameStop == true)
        {
            inputVector = Vector2.zero;
        }
        return inputVector;
    }

    public void OnBeginDrag(PointerEventData pointer)
    {
        if (!GameManager.Instance.IsGameStop)
        {
            controller.SetActive(true);
            // 클릭된 부분으로 컨트롤 스틱 UI 위치 이동
            rect.position = pointer.position;
        }
        else
        {
            controller.SetActive(false);

        }

    }

    public void OnDrag(PointerEventData pointer)
    {
        if (!GameManager.Instance.IsGameStop)
        {
            SetStickVector(pointer);
        }
        else
        {
            controller.SetActive(false);

        }

    }

    public void OnEndDrag(PointerEventData pointer)
    {
        if (!GameManager.Instance.IsGameStop)
        {
            inputVector = Vector2.zero;

            stick.anchoredPosition = Vector2.zero;
            controller.SetActive(false);
        }
        else
        {
            controller.SetActive(false);
        }
    }
}
