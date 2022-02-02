using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHatchController : MonoBehaviour
{
    public Transform RightHatch;
    public Transform LeftHatch;
    public Transform PivotRight;
    public Transform PivotLeft;

    public float OpenAngle = 90f;
    public float RotationSpeed = 3f;
    private float currentRotation;
    private float targetRotation = 0;
    private bool isRotating = false;

    private void Start()
    {

    }

    private void Update()
    {
        if (isRotating)
        {
            RotateHatches(Mathf.Sign(targetRotation - currentRotation) * RotationSpeed * Time.deltaTime);
            
            if(Mathf.Abs(targetRotation - currentRotation) < 1f)
            {
                RotateHatches(targetRotation - currentRotation);
                isRotating = false;
            }
        }
    }

    private void RotateHatches(float angle)
    {
        RightHatch.RotateAround(PivotRight.position, PivotRight.up, angle);
        LeftHatch.RotateAround(PivotLeft.position, PivotLeft.up, -angle);
        currentRotation += angle;
    }

    public void Open()
    {
        Debug.Log("Open robot hatch");
        targetRotation = OpenAngle;
        isRotating = true;
        this.CallWithDelay(Close, 2);
    }

    public void Close()
    {
        Debug.Log("Close robot hatch");
        targetRotation = 0;
        isRotating = true;
    }
}
