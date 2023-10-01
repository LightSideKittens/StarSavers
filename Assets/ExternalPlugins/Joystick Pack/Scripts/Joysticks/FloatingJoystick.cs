/*using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    private Vector2 defaultPosition;

    protected override void Start()
    {
        base.Start();
        defaultPosition = background.anchoredPosition;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = defaultPosition;
        base.OnPointerUp(eventData);
    }
}*/