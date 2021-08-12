using UnityEngine;
using UnityEngine.InputSystem;

public class EditorCamera : MonoBehaviour
{
    public Transform Target { get; set; }

    private Camera _camera;

    public float ZoomSpeed = 100;
    public float Distance = 10f;
    public float SensitivityX = 4;
    public float SensitivityY = 1;

    private float currentX = 0;
    private float currentY = 0;

    public float FieldOfView { get { return _camera.fieldOfView; } }

    private Vector3 moveVelocity;
    private Vector3 targetPosition;
    private Vector2 moveVector;
    private float zoom = 0;

    private bool scrollWheelIsPressed = false;

    void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
    }

    public void ScrollWheelDown(InputAction.CallbackContext context)
    {
        scrollWheelIsPressed = true;
    }

    public void ScrollWheelUp(InputAction.CallbackContext context)
    {
        scrollWheelIsPressed = false;
    }

    public void MouseRotate(InputAction.CallbackContext context)
    {
        if (scrollWheelIsPressed)
            moveVector = context.ReadValue<Vector2>();
    }

    public void CancelMouseRotate(InputAction.CallbackContext context)
    {
        moveVector = Vector2.zero;
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

    public void CancelRotate(InputAction.CallbackContext context)
    {
        moveVector = Vector2.zero;
    }

    public void StartZoomIn()
    {
        zoom = -ZoomSpeed;
    }

    public void StartZoomOut()
    {
        zoom = ZoomSpeed;
    }

    public void EndZoomIn()
    {
        zoom = 0;
    }

    public void EndZoomOut()
    {
        zoom = 0;
    }

    private void LateUpdate()
    {
        Vector3 newTargetPosition = Target.position;
        float gridRadius = WorldEditor.Instance.GridSize / 2f;
        Vector3 centerOffset = new Vector3(gridRadius, 0, gridRadius);
        newTargetPosition += centerOffset;

        targetPosition = Vector3.SmoothDamp(targetPosition, newTargetPosition, ref moveVelocity, 1f);

        currentX += moveVector.x;
        currentY -= moveVector.y;
        currentY = Mathf.Clamp(currentY, -89, 89);

        Distance += zoom * Time.deltaTime * Distance;
        Distance = Mathf.Clamp(Distance, 0.1f, 100f);

        Vector3 dir = new Vector3(0, 0, -Distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        transform.position = targetPosition + rotation * dir;
        transform.LookAt(targetPosition);
    }
}
