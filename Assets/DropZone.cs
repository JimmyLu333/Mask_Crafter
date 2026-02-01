using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public Step2Manager manager;
    public int zoneID;
    // 0 = dish
    // 1 = nose
    // 2 = paint

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DragTool tool = eventData.pointerDrag.GetComponent<DragTool>();
        if (tool == null) return;

        manager.TryUseTool(tool, zoneID);
    }
}
