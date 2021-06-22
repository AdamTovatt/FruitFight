using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> Targets { get; set; }

    public Vector3 Offset;
    public float SmoothTime = 0.5f;

    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 160f;

    private Vector3 moveVelocity;
    private Vector3 rotateVelocity;
    private Vector3 oldCenterPoint;
    private Camera _camera;

    public float FieldOfView { get { return _camera.fieldOfView; } }

    private void Awake()
    {
        Targets = new List<Transform>();
    }

    private void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if (Targets.Count == 0)
            return;

        Vector3 centerPoint = GetCenterPoint();

        if (oldCenterPoint == Vector3.zero)
            oldCenterPoint = centerPoint;

        Rotate(centerPoint);
        Move(centerPoint);
        Zoom();

        oldCenterPoint = centerPoint;
    }

    private void Move(Vector3 centerPoint)
    {
        Vector3 newPosition = centerPoint + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref moveVelocity, SmoothTime);
    }

    private void Rotate(Vector3 centerPoint)
    {
        Vector3 lookPosition = Vector3.SmoothDamp(transform.position, centerPoint, ref rotateVelocity, SmoothTime);
        transform.rotation = Quaternion.LookRotation(lookPosition - transform.position);
    }

    private void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, newZoom, Time.deltaTime);
    }

    private float GetGreatestDistance()
    {
        Bounds bounds = new Bounds(Targets[0].position, Vector3.zero);
        for (int i = 0; i < Targets.Count; i++)
        {
            bounds.Encapsulate(Targets[i].position);
        }

        return bounds.size.x * bounds.size.x + bounds.size.z * bounds.size.z + bounds.size.y * bounds.size.y;
    }

    private Vector3 GetCenterPoint()
    {
        if (Targets.Count == 1)
            return Targets[0].position;

        Bounds bounds = new Bounds(Targets[0].position, Vector3.zero);
        for (int i = 0; i < Targets.Count; i++)
        {
            bounds.Encapsulate(Targets[i].position);
        }

        return bounds.center;
    }
}
