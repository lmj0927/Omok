using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverEventButton : Button
{
    public Action onMouseEnter;
    public Action onMouseExit;
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onMouseEnter?.Invoke();
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onMouseExit?.Invoke();
    }
}