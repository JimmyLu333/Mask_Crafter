using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragTool : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int toolID;                 // 工具编号
    public Step2Manager manager;       // 管理器（关键！！！）

    RectTransform rect;
    Canvas canvas;
    Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPos = rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (manager != null)
        {
            manager.OnToolDropped(this);
        }

        // 回原位
        rect.anchoredPosition = startPos;
    }
}
