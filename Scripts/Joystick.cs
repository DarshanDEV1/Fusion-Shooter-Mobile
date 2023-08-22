using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] float joystickRadius = 50f;
    [SerializeField] RectTransform backgroundRect;
    [SerializeField] Vector2 centerPosition;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public Quaternion Rotation { get; private set; }
    public float Angle { get; private set; }

    private void Start()
    {
        centerPosition = backgroundRect.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragPosition = eventData.position;
        Vector2 direction = (dragPosition - centerPosition).normalized;
        float handleDistance = Mathf.Min(Vector2.Distance(dragPosition, centerPosition), joystickRadius);
        transform.position = centerPosition + direction * handleDistance;

        // Calculate the angle of the drag from the center
        Vector2 localPosition = transform.localPosition;
        float angle = Mathf.Atan2(localPosition.y, localPosition.x) * Mathf.Rad2Deg;

        // Apply rotation to the handle
        transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90);

        Horizontal = GetJoystickDirection().x;
        Vertical = GetJoystickDirection().y;
        Rotation = Quaternion.Euler(0f, ((angle - 90) * -1), 0f);
        Angle = (angle - 90) * -1;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset handle position to center when the drag ends
        transform.position = centerPosition;
        transform.rotation = Quaternion.identity;

        Horizontal = 0f;
        Vertical = 0f;
    }

    // Function to get the direction of the joystick
    public Vector2 GetJoystickDirection()
    {
        Vector2 direction = ((Vector2)transform.position - centerPosition).normalized;
        return direction;
    }
}