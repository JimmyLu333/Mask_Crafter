using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragBox : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;

    private Vector2 startPos;

    // 鼻子的位置
    public RectTransform noseRect;
    public NoseStep noseStep;

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
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 计算距离
        float distance =
            Vector2.Distance(rect.position, noseRect.position);

        // 如果够近，算成功
        if (distance < 100f)
        {
            Debug.Log("放到鼻子上了");

            // 解锁
            noseStep.Unlock();
        }

        // 不管成功失败，都回原位
        rect.anchoredPosition = startPos;
    }
}
