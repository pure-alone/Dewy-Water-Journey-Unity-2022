using System;
using UnityEngine.EventSystems;

public sealed class DewyDragHandler : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<PointerEventData> Began;
    public Action<PointerEventData> Dragged;
    public Action<PointerEventData> Ended;

    public void OnBeginDrag(PointerEventData eventData) => Began?.Invoke(eventData);
    public void OnDrag(PointerEventData eventData) => Dragged?.Invoke(eventData);
    public void OnEndDrag(PointerEventData eventData) => Ended?.Invoke(eventData);
}
