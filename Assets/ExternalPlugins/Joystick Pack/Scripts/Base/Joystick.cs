using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Vector2 Direction { get; private set; }
    [SerializeField] private RectTransform area;
    [SerializeField] private RectTransform handleArea;
    [SerializeField] private RectTransform handle;
    private float maxRadius;

    protected virtual void Start()
    {
        var size = handleArea.sizeDelta;
        maxRadius = Mathf.Min(size.x, size.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        handleArea.localPosition = eventData.position;
        handle.localPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var position = ((Vector3)eventData.position - handleArea.localPosition);
        Direction = position.normalized;
        handle.localPosition = Vector3.ClampMagnitude(position, maxRadius / 3);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.localPosition = Vector3.zero;
    }
}