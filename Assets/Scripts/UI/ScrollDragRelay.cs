using System;
using UnityEngine;
using UnityEngine.EventSystems;

// 挂在 ScrollRect 所在的物体上，把"开始拖 / 松手"转发出去
public class ScrollDragRelay : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public Action OnDragBegin;
    public Action OnDragEnd;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnDragBegin != null) OnDragBegin();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnDragEnd != null) OnDragEnd();
    }
}