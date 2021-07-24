using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> Targets { get; set; }

    public Vector3 Offset;
    public float SmoothTime = 0.5f;

    public bool IsInEditor = false;
    public float MoveSpeed = 5f;
    public float SettleRadius = 10f;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 160f;

    private Vector3 moveVelocity;
    private Vector3 rotateVelocity;
    private Vector3 oldCenterPoint;
    private Camera _camera;

    private List<Vector3> targetMovements = new List<Vector3>();
    private float lastAverageMovementTime = 0;
    private Vector3 lastCenterpoint = Vector3.zero;
    private Vector3 averageMovement = Vector3.zero;
    private int movementSamples = 100;
    private float cameraRotation = 0;

    public float FieldOfView { get { return _camera.fieldOfView; } }

    private CameraHint[] cameraHints;
    private CameraHint[] intersectingCameraHints;

    private void Awake()
    {
        Targets = new List<Transform>();
    }

    private void Start()
    {
        _camera = gameObject.GetComponent<Camera>();

        for (int i = 0; i < movementSamples; i++)
        {
            targetMovements.Add(new Vector3(0, 0, 0));
        }
    }

    public void SetCameraHints(CameraHint[] hints)
    {
        cameraHints = hints;
    }

    private void Update()
    {
        if (Targets.Count == 0)
            return;

        intersectingCameraHints = cameraHints.Where(x => (x.transform.position - lastCenterpoint).sqrMagnitude < x.RadiusSquared).ToArray();

        Vector3 centerPoint = GetCenterPoint();

        if (oldCenterPoint == Vector3.zero)
            oldCenterPoint = centerPoint;

        if (lastCenterpoint != Vector3.zero)
        {
            if (Time.time - lastAverageMovementTime > 0.01f)
            {
                lastAverageMovementTime = Time.time;
                averageMovement += (centerPoint - lastCenterpoint);
                averageMovement *= 0.99f;
                lastCenterpoint = centerPoint;
            }
        }
        else
        {
            lastCenterpoint = centerPoint;
        }

        Rotate(centerPoint);

        if (IsInEditor)
        {
            MoveEditor(centerPoint);
        }
        else
        {
            if ((new Vector3(centerPoint.x, (centerPoint + Offset).y, centerPoint.z) - transform.position).sqrMagnitude > SettleRadius * SettleRadius)
                Move(centerPoint);
            else
                Move(new Vector3(transform.position.x, (centerPoint + Offset).y, transform.position.z));
        }

        Zoom();

        oldCenterPoint = centerPoint;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public void RotateCamera(float degrees)
    {
        cameraRotation = degrees;
    }

    private void Move(Vector3 centerPoint)
    {
        float weight = (Mathf.Min(averageMovement.sqrMagnitude, 20) / 20f);
        Vector3 aboveCenter = new Vector3(centerPoint.x, (centerPoint + Offset).y, centerPoint.z);

        Vector3 targetPosition = aboveCenter - (averageMovement * weight);
        foreach (CameraHint hint in intersectingCameraHints)
        {
            float hintWeight = (hint.transform.position - lastCenterpoint).sqrMagnitude / hint.RadiusSquared;
            targetPosition += (hint.transform.position - transform.position) * hintWeight;
        }

        targetPosition += transform.right * cameraRotation; //RotatePointAroundPivot(targetPosition, centerPoint, new Vector3(0, cameraRotation * Time.deltaTime, 0));

        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * MoveSpeed * 200);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref moveVelocity, SmoothTime);
    }

    private void MoveEditor(Vector3 centerPoint)
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
